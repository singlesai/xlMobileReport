using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace xl_rp
{
    class HttpServer
    {
        private HttpListener _listener;
        private string _domain;
        private int _port;
        private Thread _thread;
        private string _staticPath;

        public delegate void StartHandler(string domain,int port);
        public event StartHandler startHandler;

        public delegate void StopHandler(string domain, int port);
        public event StopHandler stopHandler;

        public delegate void ReauestHandler(HttpListenerContext ctx);
        public event ReauestHandler requestHandler;

        public int port
        {
            get { return _port; }
            set { _port = value; }
        }

        public string domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        public string staticPath
        {
            get { return _staticPath; }
            set { _staticPath = value; }
        }

        public HttpServer(string domain="127.0.0.1",int port = 80)
        {
            _listener = new HttpListener();
            _domain = domain;
            _port = port;
        }

        public void start()
        {
            _listener.Prefixes.Add(string.Format("http://{0}:{1}/", _domain, _port));
            _listener.Start();
            if(startHandler!=null)
                startHandler(_domain, _port);
            int minThreadNum;
            int portThreadNum;
            int maxThreadNum;
            ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);
            ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);

            _thread = new Thread(new ThreadStart(Listening));
            _thread.IsBackground = true;
            _thread.Start();
            /*
            while (true)
            {
                HttpListenerContext ctx = _listener.GetContext();
                ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc), ctx);
            }
            */
        }

        void Listening()
        {
                while (true)
                {
                try
                {
                    HttpListenerContext ctx = _listener.GetContext();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc), ctx);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
        }

        void TaskProc(object o)
        {
            try
            {
                HttpListenerContext ctx = (HttpListenerContext)o;
                if (!string.IsNullOrEmpty(_staticPath))
                {
                    HttpListenerRequest request = ctx.Request;
                    HttpListenerResponse response = ctx.Response;
                    response.ContentEncoding = Encoding.UTF8;
                    response.ContentType = "text/html;charset=utf-8";

                    var path = _staticPath;
                    var fileName = _staticPath + request.Url.LocalPath;

                    if (File.Exists(fileName))
                    {
                        if (Path.HasExtension(fileName))
                        {
                            string ext = Path.GetExtension(fileName);
                            switch (ext.ToLower())
                            {
                                case ".html":
                                case ".htm":
                                    response.ContentType = "text/html;charset=utf-8";
                                    break;
                                case ".js":
                                    response.ContentType = "text/javascript;charset=utf-8";
                                    break;
                                case ".css":
                                    response.ContentType = "text/css;charset=utf-8";
                                    break;
                                default:
                                    response.ContentType = "application/octet-stream;charset=utf-8";
                                    break;
                            }
                        }
                        var buff = File.ReadAllBytes(fileName);
                        response.ContentLength64 = buff.Length;

                        Stream output = response.OutputStream;
                        output.Write(buff, 0, buff.Length);
                        output.Close();
                    }
                    else
                    {
                        if (requestHandler != null)
                            requestHandler(ctx);
                    }
                }
                else
                {
                    if (requestHandler != null)
                        requestHandler(ctx);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public void stop()
        {
            _thread.Abort();
            _listener.Stop();
            if(stopHandler!=null)
                stopHandler(_domain, _port);
        }
    }
}
