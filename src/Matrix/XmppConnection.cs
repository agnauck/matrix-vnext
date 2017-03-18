﻿using System;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Matrix.Network;
using Matrix.Network.Codecs;
using Matrix.Network.Handlers;
using Matrix.Xml;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Stream;
using System.Threading;
using System.Reactive.Subjects;

namespace Matrix
{
    public abstract class XmppConnection
    {
        protected   Bootstrap                   Bootstrap              = new Bootstrap();        
        readonly    MultithreadEventLoopGroup   eventLoopGroup         = new MultithreadEventLoopGroup();
        
        readonly    XmppStreamEventHandler      xmppStreamEventHandler = new XmppStreamEventHandler();
        private     INameResolver               resolver               = new DefaultNameResolver();


        protected XmppConnection() 
            : this(null)
        {
        }      
        
        protected XmppConnection(Action<IChannelPipeline> pipelineInitializerAction)
        {            

            Bootstrap
                .Group(eventLoopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.SoKeepalive, true)
                .Resolver(HostnameResolver)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    Pipeline = channel.Pipeline;
                    
                    
                    Pipeline.AddLast(new ZlibDecoder());
                   
                    Pipeline.AddLast(new KeepAliveHandler());                   

                    
                    Pipeline.AddLast(new XmlStreamDecoder());
                                        
                    Pipeline.AddLast(new ZlibEncoder());
                    Pipeline.AddLast(new XmppXElementEncoder());
                    Pipeline.AddLast(new UTF8StringEncoder());

                    //Pipeline.AddLast(xmppStreamEventHandler);
                    Pipeline.AddLast(new AutoReplyToPingHandler<Iq>());
                    Pipeline.AddLast(xmppStreamEventHandler);

                    Pipeline.AddLast(new StreamFooterHandler());
                    
                    Pipeline.AddLast(XmppStanzaHandler);

                    Pipeline.AddLast(CatchAllXmppStanzaHandler.Name, new CatchAllXmppStanzaHandler());
                    
                    Pipeline.AddLast(new DisconnectHandler());

                    pipelineInitializerAction?.Invoke(Pipeline);                    
                }));
        }

        #region << Properties >>
        public IChannelPipeline Pipeline { get; protected set; } = null;
        

        public XmppSessionState XmppSessionState { get; } = new XmppSessionState();

        public string XmppDomain { get; set; }

        public int Port { get; set; } = 5222;

        public ICertificateValidator CertificateValidator { get; set; } = new DefaultCertificateValidator();

        public IObservable<XmppXElement> XmppXElementStream => xmppStreamEventHandler.XmppXElementStream;

        private IObservable<XmlStreamEvent> XmlStreamEvent => xmppStreamEventHandler.XmlStreamEvent;

        private readonly XmppStanzaHandler XmppStanzaHandler = new XmppStanzaHandler();
        
        public INameResolver HostnameResolver
        {
            get { return resolver; }
            set
            {
                resolver = value;
                Bootstrap.Resolver(resolver);
            }
        }
        #endregion

        #region << Send members >>

        #region << SendAsync string members >>
        protected async Task SendAsync(string data)
        {
            await Pipeline.WriteAndFlushAsync(data);
        }
       
        protected async Task<T> SendAsync<T>(string data)
            where T : XmppXElement
        {
            return await SendAsync<T>(data, XmppStanzaHandler.DefaultTimeout);
        }
        protected async Task<T> SendAsync<T>(string data, int timeout)
           where T : XmppXElement
        {            
            return await XmppStanzaHandler.SendAsync<T>(data, timeout);
        }

        protected async Task<T> SendAsync<T>(string data, CancellationToken cancellationToken)
           where T : XmppXElement
        {
            return await SendAsync<T>(data, XmppStanzaHandler.DefaultTimeout, cancellationToken);
        }

        protected async Task<T> SendAsync<T>(string data, int timeout , CancellationToken cancellationToken)
           where T : XmppXElement
        {
            return await XmppStanzaHandler.SendAsync<T>(data, timeout, cancellationToken);
        }

        protected async Task<XmppXElement> SendAsync<T1, T2>(string data)
            where T1 : XmppXElement
            where T2 : XmppXElement
        {
            return await SendAsync<T1, T2>(data, XmppStanzaHandler.DefaultTimeout);
        }

        protected async Task<XmppXElement> SendAsync<T1, T2>(string data, int timeout)
            where T1 : XmppXElement
            where T2 : XmppXElement
        {
            return await XmppStanzaHandler.SendAsync<T1, T2>(data, timeout);
        }

        protected async Task<XmppXElement> SendAsync<T1, T2>(string data, CancellationToken cancellationToken)
          where T1 : XmppXElement
          where T2 : XmppXElement
        {
            return await SendAsync<T1, T2>(data, XmppStanzaHandler.DefaultTimeout, cancellationToken);
        }

        protected async Task<XmppXElement> SendAsync<T1, T2>(string data, int timeout, CancellationToken cancellationToken)
           where T1 : XmppXElement
           where T2 : XmppXElement
        {
            return await XmppStanzaHandler.SendAsync<T1, T2>(data, timeout, cancellationToken);
        }
        #endregion

        #region << SendAsync XmppXElement members >>
        public async Task SendAsync(XmppXElement el)
        {
            await SendAsync(el.ToString(false));
        }

        public async Task<T> SendAsync<T>(XmppXElement el)
             where T : XmppXElement
        {
            return await SendAsync<T>(el, XmppStanzaHandler.DefaultTimeout);
        }

        public async Task<T> SendAsync<T>(XmppXElement el, int timeout)
             where T : XmppXElement
        {
            return await XmppStanzaHandler.SendAsync<T>(el, timeout);
        }

        public async Task<T> SendAsync<T>(XmppXElement el, CancellationToken cancellationToken)
             where T : XmppXElement
        {
            return await SendAsync<T>(el, XmppStanzaHandler.DefaultTimeout, cancellationToken);
        }

        public async Task<T> SendAsync<T>(XmppXElement el, int timeout, CancellationToken cancellationToken)
             where T : XmppXElement
        {
            return await XmppStanzaHandler.SendAsync<T>(el, timeout, cancellationToken);
        }

        public async Task<XmppXElement> SendAsync<T1, T2>(XmppXElement el)
            where T1 : XmppXElement
            where T2 : XmppXElement
        {
            return await SendAsync<T1, T2>(el, XmppStanzaHandler.DefaultTimeout);
        }

        public async Task<XmppXElement> SendAsync<T1, T2>(XmppXElement el, int timeout)
            where T1 : XmppXElement
            where T2 : XmppXElement
        {
            return await XmppStanzaHandler.SendAsync<T1, T2>(el, timeout);
        }

        public async Task<XmppXElement> SendAsync<T1, T2>(XmppXElement el, CancellationToken cancellationToken)
            where T1 : XmppXElement
            where T2 : XmppXElement
        {
            return await SendAsync<T1, T2>(el, XmppStanzaHandler.DefaultTimeout, cancellationToken);
        }

        public async Task<XmppXElement> SendAsync<T1, T2>(XmppXElement el, int timeout, CancellationToken cancellationToken)
            where T1 : XmppXElement
            where T2 : XmppXElement
        {
            return await XmppStanzaHandler.SendAsync<T1, T2>(el, timeout, cancellationToken);
        }

        public async Task<XmppXElement> SendAsync<T1, T2, T3>(XmppXElement el)
            where T1 : XmppXElement
            where T2 : XmppXElement
            where T3 : XmppXElement
        {
            return await SendAsync<T1, T2, T3>(el, XmppStanzaHandler.DefaultTimeout);
        }

        public async Task<XmppXElement> SendAsync<T1, T2, T3>(XmppXElement el, int timeout)
            where T1 : XmppXElement
            where T2 : XmppXElement
            where T3 : XmppXElement
        {
            return await XmppStanzaHandler.SendAsync<T1, T2, T3>(el, timeout);
        }

        public async Task<XmppXElement> SendAsync<T1, T2, T3>(XmppXElement el, CancellationToken cancellationToken)
            where T1 : XmppXElement
            where T2 : XmppXElement
            where T3 : XmppXElement
        {
            return await SendAsync<T1, T2, T3>(el, XmppStanzaHandler.DefaultTimeout, cancellationToken);
        }

        public async Task<XmppXElement> SendAsync<T1, T2, T3>(XmppXElement el, int timeout, CancellationToken cancellationToken)
            where T1 : XmppXElement
            where T2 : XmppXElement
            where T3 : XmppXElement
        {
            return await XmppStanzaHandler.SendAsync<T1, T2, T3>(el, timeout, cancellationToken);
        }
        #endregion

        #endregion

        public async Task<StreamFeatures> ResetStreamAsync(CancellationToken cancellationToken)
        {            
            Pipeline.Get<XmlStreamDecoder>().Reset();            
            return await SendStreamHeaderAsync(cancellationToken);
        }

        protected async Task<StreamFeatures> SendStreamHeaderAsync()
        {
            return await SendStreamHeaderAsync(XmppStanzaHandler.DefaultTimeout);
        }

        /// <summary>
        /// Sends the XMPP stream header and awaits the reply.
        /// </summary>
        /// <exception cref="StreamErrorException">
        /// Throws a StreamErrorException when the server returns a stream error
        /// </exception>
        /// <returns></returns>
        protected async Task<StreamFeatures> SendStreamHeaderAsync(int timeout)
        {
            return await SendStreamHeaderAsync(timeout, CancellationToken.None);
        }

        protected async Task<StreamFeatures> SendStreamHeaderAsync(CancellationToken cancellationToken)
        {
            return await SendStreamHeaderAsync(XmppStanzaHandler.DefaultTimeout, cancellationToken);
        }

        protected async Task<StreamFeatures> SendStreamHeaderAsync(int timeout, CancellationToken cancellationToken)
        {
            var streamHeader = new Stream
            {
                To = new Jid(XmppDomain),
                Version = "1.0"
            };

            var res = await SendAsync<StreamFeatures, Xmpp.Stream.Error>(streamHeader.StartTag(), timeout, cancellationToken);

            if (res.OfType<StreamFeatures>())
                return res.Cast<StreamFeatures>();
            else //if (res.OfType<Xmpp.Stream.Error>())
                throw new StreamErrorException(res.Cast<Xmpp.Stream.Error>());
        }


        public async Task<bool> CloseAsync(int timeout = 2000)
        {
            IDisposable anonymousSubscription = null;
            var resultCompletionSource = new TaskCompletionSource<bool>();
            
            await SendAsync(new Stream().EndTag());

            anonymousSubscription = XmppXElementStream.Subscribe(
                v => { },

                () =>
                {
                    anonymousSubscription?.Dispose();
                    ////if (Pipeline.Channel.Open)
                    ////    await Pipeline.CloseAsync();
                    resultCompletionSource.SetResult(true);
                    
                });

           
            if (resultCompletionSource.Task ==
                await Task.WhenAny(resultCompletionSource.Task, Task.Delay(timeout)))
            {
                await TryCloseAsync();
                return await resultCompletionSource.Task;
            }
                

            // timed out
            anonymousSubscription.Dispose();
            await TryCloseAsync();

            return true;
        }

        private async Task TryCloseAsync()
        {
            if (Pipeline.Channel.Active)
                await Pipeline.CloseAsync();

            XmppSessionState.Value = SessionState.Disconnected;
        }
    }
}
