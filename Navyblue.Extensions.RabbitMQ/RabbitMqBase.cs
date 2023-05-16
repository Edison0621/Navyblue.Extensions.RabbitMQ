using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Extensions.Options;
using Navyblue.Extensions.RabbitMQ.Setting;
using RabbitMQ.Client;

namespace Navyblue.Extensions.RabbitMQ
{
    public abstract class RabbitMqBase
    {
        private readonly RabbitMqSetting _setting;
        private static readonly ConcurrentDictionary<string, Lazy<IConnection>> ConcurrentDictionary = new ConcurrentDictionary<string, Lazy<IConnection>>();
        protected readonly Dictionary<string, RabbitMqQueueSetting> _rabbitMqQueueSettings;

        protected RabbitMqBase(IOptions<RabbitMqSetting> setting)
        {
            this._setting = setting.Value;
            this._rabbitMqQueueSettings = this._setting.Queues.ToDictionary(k => k.MessageType, v => v);
        }

        public IConnection GetConnection(string hostName)
        {
            if (ConcurrentDictionary.TryGetValue(hostName, out Lazy<IConnection> lazyConnection) && lazyConnection.Value.IsOpen)
            {
                return lazyConnection.Value;
            }

            IConnection connection = ConcurrentDictionary.AddOrUpdate(hostName,
                newValue => new Lazy<IConnection>(this.CreateConnection, LazyThreadSafetyMode.ExecutionAndPublication),
                (existName, newValue) => new Lazy<IConnection>(this.CreateConnection, LazyThreadSafetyMode.ExecutionAndPublication)).Value;

            return connection;
        }

        private IConnection CreateConnection()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = this._setting.Host,
                UserName = this._setting.SslEnabled ? string.Empty : this._setting.Username,
                Password = this._setting.SslEnabled ? string.Empty : this._setting.Password,
                Port = this._setting.Port,
                VirtualHost = this._setting.VirtualHost
            };

            factory.Ssl = this._setting.SslEnabled ? new SslOption
            {
                Enabled = true,
                ServerName = factory.HostName,
                AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors,
                CertificateSelectionCallback = this.GetCertificateSelection,
                Version = SslProtocols.Tls12
            } : factory.Ssl;

            factory.AuthMechanisms = this._setting.SslEnabled ? new IAuthMechanismFactory[]
            {
                new ExternalMechanismFactory()
            } : ConnectionFactory.DefaultAuthMechanisms;

            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);
            factory.TopologyRecoveryEnabled = true;

            return factory.CreateConnection();
        }

        private X509Certificate GetCertificateSelection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                X509FindType certFindType = X509FindType.FindBySubjectName;
                string certFindValue = this._setting.CertSubject;
                X509Certificate2Collection certificateCol = store.Certificates.Find(certFindType, certFindValue, true);
                store.Close();
                return certificateCol[0];
            }
        }
    }
}