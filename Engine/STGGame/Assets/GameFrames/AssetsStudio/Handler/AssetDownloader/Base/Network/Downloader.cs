using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using UnityEngine;

///////////////////////////////////////////////////////////
//
//  Written by              ：laishikai
//  Copyright(C)            ：
//  ------------------------------------------------------
//  功能描述                ：Http 下载管理器的接口类
//
///////////////////////////////////////////////////////////
namespace ASGame
{
    public class DownResInfo
    {
        public string url;          // 资源文件的URL
        public string savePath;     // 保存的路径
        public int nDownSize;       // 已经下载的大小(字节B),断点续传
        public int nFileSize;       // 文件大小(字节B)
    };

    public class DownResFile
    {
        public string url;          // 资源文件的URL
        public int nFileSize;
        public FileStream file;
    }

    public delegate void DownloadProgress(long curSize, long totalSize);
    public delegate void DownloadFinish(string url, string path);

    public class CDownloader
    {
        List<DownResInfo> m_downList;
        List<DownResFile> m_successDownList;
        List<DownResFile> m_failedDownList;

        int m_nNextDownIndex = 0;           //当前下载下标

        int m_nHadDownedCount = 0;          //下载总数量
        int m_nTotalNeedDownCount = 0;      //下载总数量

        int m_nDownThreadNumb = 0;          // 下载线程数量
        int m_nWriteThreadNumb = 0;
        bool m_bNeedStop = false;
        bool m_bIsPause = false;
        Thread[] m_runThreads;
        Thread m_runWriteThread;

        ManualResetEvent m_pauseEvent;

        long m_nDownSize;                   // 当前下载的大小
        long m_nHadDownedSize;              // 当前总的下载大小
        long m_nTotalNeedDownSize;          // 当前下载总的下载量
        long m_nLimitDownSize;              // 每秒限制下载的大小
        long m_nLastTime;                   // 上一次统计的时间点


        DownloadFinish m_downloadFinish;
        DownloadProgress m_downloadProgress;

        public long HadDownedSize { get { return m_nHadDownedSize; } }
        public long TotalNeedDownSize { get { return m_nTotalNeedDownSize; } }
        public int HadDownedCount { get { return m_nHadDownedCount; } }
        public int TotalNeedDownCount { get { return m_nTotalNeedDownCount; } }
        public DownloadFinish OnDownloadFinish { get { return m_downloadFinish; } set { m_downloadFinish = value; } }
        public DownloadProgress OnDownloadProgress { get { return m_downloadProgress; } set { m_downloadProgress = value; } }
        public List<DownResFile> SuccessDownList { get => m_successDownList; }
        public List<DownResFile> FailedDownList { get => m_failedDownList; }

        public CDownloader()
        {
            m_pauseEvent = new ManualResetEvent(false);
            m_successDownList = new List<DownResFile>();
            m_failedDownList = new List<DownResFile>();
        }

        public DownResInfo CreateDownInfo(string url,string path)
        {
            DownResInfo node = new DownResInfo();
            node.url = url;
            node.savePath = path;
            CHttpDown.GetDownFileSize(url, out node.nFileSize);

            return node;
        }

        public List<DownResInfo> CreateDownList(string[] downUrls, string[] savePaths)
        {
            List<DownResInfo> downList = new List<DownResInfo>();
            downList.Clear();
            if (downUrls != null && downUrls.Length > 0)
            {
                for (int i = 0; i < downUrls.Length; i++)
                {
                    var url = downUrls[i];
                    var path = savePaths[i];
                    var resInfo = CreateDownInfo(url, path);

                    downList.Add(resInfo);
                }
            }

            return downList;
        }

        // 功能：开启多线程下载
        // 参数：downList - 下载列表
        //      nDownThreadNumb - 下载线程数量
        //      nLimitDownSize - 下载速度限制
        public void StartDown(List<DownResInfo> downList, int nDownThreadNumb, int nLimitDownSize)
        {
            if (m_bIsPause)
            {
                m_bIsPause = false;
                m_pauseEvent.Set();
                return;
            }

            m_nDownSize = 0;
            m_nHadDownedSize = 0;
            m_nTotalNeedDownSize = 0;
            m_nLimitDownSize = nLimitDownSize;
            m_nLastTime = 0;

            // 统计总的下载量
            for (int i = downList.Count - 1; i >= 0; --i)
            {
                m_nTotalNeedDownSize += downList[i].nFileSize;
            }

            if (nDownThreadNumb > downList.Count)
                nDownThreadNumb = downList.Count;

            m_downList = downList;
            m_successDownList.Clear();
            m_failedDownList.Clear();

            m_nNextDownIndex = 0;

            m_nTotalNeedDownCount = downList.Count;
            m_nHadDownedCount = 0;

            m_nDownThreadNumb = nDownThreadNumb;
            m_pauseEvent.Set();
            m_runThreads = new Thread[nDownThreadNumb];

            for (int i = 0; i < nDownThreadNumb; ++i)
            {
                Thread t = new Thread(ThreadFunc);
                t.Start(this);
                m_runThreads[i] = t;
            }

            // 启动写线程
            m_nWriteThreadNumb = 1;
            Thread tw = new Thread(WriteThreadFunc);
            tw.Start(this);
            m_runWriteThread = tw;
        }

        public void StartDown(string[] urlList, string[] savePaths, int nDownThreadNumb, int nLimitDownSize)
        {
            List<DownResInfo> downList = CreateDownList(urlList, savePaths);
            StartDown(downList, nDownThreadNumb, nLimitDownSize);
        }

        public void PauseDown()
        {
            m_bIsPause = true;

            m_pauseEvent.Reset();
        }

        public void StopDown(bool bAbort, bool bWait)
        {
            m_bIsPause = false;
            m_pauseEvent.Set();

            if (m_nDownThreadNumb > 0)
            {
                m_bNeedStop = true;
                m_runThreads = null;
            }
            if (!bAbort && bWait)
            {
                while (m_nDownThreadNumb > 0 || m_nWriteThreadNumb > 0)
                {
                    Thread.Sleep(1); // 强制等待线程退出
                }
            }
        }

        public void ClearDown()
        {
            m_bIsPause = false;
            m_pauseEvent.Set();

            m_downloadFinish = null;
            m_downloadProgress = null;

            m_successDownList.Clear();
            m_failedDownList.Clear();
        }

        static void ThreadFunc(object obj)
        {
            CDownloader pMng = obj as CDownloader;
            pMng.DownThread();
        }

        static void WriteThreadFunc(object obj)
        {
            CDownloader pMng = obj as CDownloader;
            pMng.WriteThread();
        }

        bool PopDownFileInfo(out DownResInfo resInfo)
        {
            resInfo = null;
            if (m_bNeedStop)
                return false;
            lock (this)
            {
                if (m_nNextDownIndex < m_nTotalNeedDownCount)
                {
                    resInfo = m_downList[m_nNextDownIndex++];
                }
            }
            return resInfo != null;
        }

        void DownThread()
        {
            DownResInfo resInfo = null;
            CHttp http = new CHttp();
            while (!m_bNeedStop)
            {
                m_pauseEvent.WaitOne();

                if (PopDownFileInfo(out resInfo))
                {
                    DownFile(http, resInfo);
                }
                else
                    break;
            }
            http.Close();
            // 线程退出，线程数减一
            System.Threading.Interlocked.Decrement(ref m_nDownThreadNumb);
        }

        bool IsNeedLimitDown()
        {
            lock (this)
            {
                if (m_nLimitDownSize > 0 && m_nDownSize > m_nLimitDownSize)
                {
                    long nNow = System.DateTime.Now.Ticks / 10000;
                    if (0 == m_nLastTime)
                        m_nLastTime = nNow;
                    long nPassTime = nNow - m_nLastTime;
                    if (nPassTime < 1)
                        nPassTime = 1;
                    return m_nDownSize * 1000 / nPassTime > m_nLimitDownSize;
                }
            }
            return false;
        }

        void LimitSpeed()
        {
            while (IsNeedLimitDown())
            {
                Thread.Sleep(10);
            }
        }

        void DownFile(CHttp http, DownResInfo resInfo)
        {
            string url = resInfo.url;
            int nFileSize = resInfo.nFileSize;
            int nLastDownSize = resInfo.nDownSize;

            // 如果文件比较小的话，可以不分片下载，真正下载整个文件
            if (nFileSize == 0)
                CHttpDown.GetDownFileSize(url, out nFileSize);

            DownResFile resFile = new DownResFile();
            resFile.url = url;
            resFile.nFileSize = 0;

            if (0 == nFileSize)
            {
                // 无法获取文件大小信息,整个下载吧
                bool bSuc = DownPart(http, url, 0, 0, nFileSize, resInfo, resFile);
                NotifyDownEvent(url, bSuc, resInfo, resFile);
                return;
            }
            int nPageSize = 1024 * 300;         // 分片的大小，应小于你的最大限制下载速度, 这里默认选用300K，读者自己根据项目修改
            int nFileOffset = nLastDownSize;    // 从上一次下载的位置接着下载
            int nDownSize = 0;
            for (; nFileOffset < nFileSize; nFileOffset += nPageSize)
            {
                // 先限速
                LimitSpeed();
                // 开始分片下载
                nDownSize = nFileOffset + nPageSize < nFileSize ? nPageSize : (nFileSize - nFileOffset);
                if (!DownPart(http, url, nFileOffset, nDownSize, nFileSize, resInfo, resFile))
                {
                    NotifyDownEvent(url, false, resInfo , resFile);
                    return;
                }
            }

            NotifyDownEvent(url, true, resInfo, resFile);
        }

        // 功能：通知文件下载事件
        void NotifyDownEvent(string url, bool bSuc, DownResInfo resInfo , DownResFile resFile)
        {
            // 这里只是输出一个日志，用户自行扩展事件吧
            MemBlock pBlock = new MemBlock();
            pBlock.resFile = resFile;
            PushWrite(pBlock); // 通知写线程关闭对应的文件
            m_nHadDownedCount++;

            OnFileDownloadComplete(url, bSuc, resInfo, resFile);
           
        }

        public class MemBlock
        {
            public MemBlock m_pNext;
            public string url;
            public byte[] data;
            public int nFileOfset;
            public int nDownSize;
            public int nFileSize; // 文件总的大小

            public DownResInfo resInfo;
            public DownResFile resFile;
        }

        MemBlock m_InvalidBlock;
        int m_nCurBlockMemSize;
        int m_nUseBlockMemSize; // 当前使用的内存
        MemBlock m_WriteList;  // 需要写的的队列
        MemBlock AllockBlock(string url, int nFileOffset, int nDownSize, int nFileSize)
        {
            MemBlock pBlock = null;
            lock (this)
            {
                pBlock = m_InvalidBlock;
                if (m_InvalidBlock != null)
                {
                    m_nUseBlockMemSize += 4096;
                    m_InvalidBlock = m_InvalidBlock.m_pNext;
                }
            }
            if (pBlock == null)
            {
                pBlock = new MemBlock();
                pBlock.data = new byte[4096]; // 固定4K
                lock (this)
                {
                    m_nCurBlockMemSize += 4096;
                }
            }
            pBlock.url = url;
            pBlock.nFileOfset = nFileOffset;
            pBlock.nDownSize = nDownSize;
            pBlock.nFileSize = nFileSize;
            pBlock.resFile = null;
            pBlock.resInfo = null;
            return pBlock;
        }
        void FreeBlock(MemBlock pBlock)
        {
            if (pBlock.data == null)
                return;
            pBlock.resFile = null;
            pBlock.resInfo = null;
            lock (this)
            {
                m_nUseBlockMemSize -= 4096;
                pBlock.m_pNext = m_InvalidBlock;
                m_InvalidBlock = pBlock;
            }
        }
        void WaitBlock(int nMaxSize)
        {
            while (m_nUseBlockMemSize >= nMaxSize)  // 这里只引用，不修改，所以不加锁，虽然并不一定准确
            {
                Thread.Sleep(10); // 等一下吧
            }
        }
        bool DownPart(CHttp http, string url, int nFileOffset, int nDownSize, int nFileSize, DownResInfo resInfo, DownResFile resFile)
        {
            // 调用Http下载的代码
            nDownSize = http.PrepareDown(url, nFileOffset, nDownSize, nDownSize == 0);
            if (nDownSize <= 0)
            {
                Debug.LogError("文件下载失败，url:" + url + "(" + nFileOffset + "-" + nDownSize + ")");
                return false;
            }

            // 将下载的内容提交到写线程
            byte[] szTempBuf = null;
            int nCurDownSize = 0;
            int nRecTotal = 0;
            int nRecLen = 0;
            int nOffset = 0;
            nCurDownSize = nDownSize > 4096 ? 4096 : nDownSize;
            MemBlock pBlock = AllockBlock(url, nFileOffset, nCurDownSize, nFileSize);  // 从内存池中取一个4K的内存片
            while (nDownSize > 0 && !m_bNeedStop)
            {
                // 必要的话，在这里添加限速功能或限制接收速度的功能，以免网速太快，导致一秒内分配太多内存
                //LimitSpeed();
                nRecLen = http.FastReceiveMax(ref szTempBuf, ref nOffset, 4096 - nRecTotal);
                if (nRecLen > 0)
                {
                    OnReceive(nRecLen);  // 统计下载的流量
                    Array.Copy(szTempBuf, nOffset, pBlock.data, nRecTotal, nRecLen);
                    nRecTotal += nRecLen;
                    // 如果当前块接收满了
                    if (nRecTotal >= nCurDownSize)
                    {
                        pBlock.resFile = resFile;
                        pBlock.resInfo = resInfo;
                        PushWrite(pBlock);// 提交写文件
                        nRecTotal = 0;
                        nDownSize -= nCurDownSize;
                        nFileOffset += nCurDownSize;
                        nCurDownSize = nDownSize > 4096 ? 4096 : nDownSize;
                        // 必要的话，加上限额等待
                        if (nCurDownSize > 0)
                        {
                            WaitBlock(1024 * 1024); // 检测当前内存池分配的总量，超过就挂起
                            pBlock = AllockBlock(url, nFileOffset, nCurDownSize, nFileSize);  // 从内存池中取一个4K的内存片
                        }
                    }
                }
                else
                {
                    return false; // 文件读取失败，可能是网络出问题了
                }
            }
            return true;
        }
        void OnReceive(int nDownSize)
        {
            // 统计下载量，下载进度
            long nNow = System.DateTime.Now.Ticks / 10000;
            lock (this)
            {
                if (0 == m_nLastTime)
                    m_nLastTime = nNow;
                long nPassTime = nNow - m_nLastTime;
                if (nPassTime > 1000)
                {
                    m_nLastTime = nNow - nPassTime % 1000;
                    m_nDownSize = (m_nDownSize + nDownSize) * (nPassTime % 1000) / nPassTime;
                }
                else
                    m_nDownSize += nDownSize;
                m_nHadDownedSize += nDownSize;

                OnFileDownloadProgress(m_nHadDownedSize, m_nTotalNeedDownSize);
            }
        }
        void PushWrite(MemBlock pBlock)
        {
            lock (this)
            {
                pBlock.m_pNext = m_WriteList;
                m_WriteList = pBlock;
            }
        }
        // 功能：反转单链表
        MemBlock Reverse(MemBlock pBlock)
        {
            MemBlock pList = null;
            MemBlock pNext = null;
            while (pBlock != null)
            {
                pNext = pBlock.m_pNext;
                pBlock.m_pNext = pList;
                pList = pBlock;
                pBlock = pNext;
            }
            return pList;
        }
        // 功能：写线程
        void WriteThread()
        {
            while (!m_bNeedStop)
            {
                MemBlock pList = null;
                lock (this)
                {
                    pList = m_WriteList;
                    m_WriteList = null;
                }
                if (pList == null)
                {
                    if (m_nDownThreadNumb <= 0)
                        break;
                    Thread.Sleep(1); // 没有要写的文件，小睡一会，减少CPU的开销
                    continue;
                }
                pList = Reverse(pList);
                // 开始写入文件吧
                MemBlock pBlock = null;
                while (pList != null)
                {
                    pBlock = pList;
                    pList = pList.m_pNext;
                    SafeWriteBlock(pBlock);  // 写入文件
                    FreeBlock(pBlock); // 回收内存
                }
            }
            m_InvalidBlock = null; // 不需要内存池了
                                   // 在这里通知主线程，下载结束

            // 线程退出，线程数减一
            System.Threading.Interlocked.Decrement(ref m_nWriteThreadNumb);
        }
        void SafeWriteBlock(MemBlock pBlock)
        {
            try
            {
                WriteBlock(pBlock);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void WriteBlock(MemBlock pBlock)
        {
            // 这里可以使用一个特殊的标记，表示文件下载完成的
            if (pBlock.data == null)
            {
                // 在这里添加事件处理
                if (pBlock.resFile != null)
                {
                    if (pBlock.resFile.file != null)
                    {
                        pBlock.resFile.file.Close();
                        pBlock.resFile.file = null;
                    }
                    pBlock.resFile = null;
                }
                return;
            }
            // 必要的话，在这里记录一下下载的状态, 这个用户自行扩展吧
            DownResFile resFile = pBlock.resFile;
            DownResInfo resInfo = pBlock.resInfo;
            if (resFile.file == null)
            {
                string szLocalPathName = resInfo.savePath;
                if (pBlock.nFileOfset == 0)
                {
                    if (File.Exists(szLocalPathName))
                        File.Delete(szLocalPathName);
                }
                resFile.file = new FileStream(szLocalPathName, FileMode.OpenOrCreate, FileAccess.Write);
                if (resFile.file == null)
                {
                    Debug.LogError(szLocalPathName + "文件打开失败!");
                }
            }
            FileStream file = resFile.file;
            if (file != null)
            {
                file.Seek(pBlock.nFileOfset, SeekOrigin.Begin);
                file.Write(pBlock.data, 0, pBlock.nDownSize);
                file.Flush();
            }
        }

        ///
        // FIXME:子线程不应该直接调用回调,不然会卡住子线程
        private void OnFileDownloadProgress(long nHadDownedSize, long nTotalNeedDownSize)
        {
            m_downloadProgress?.Invoke(nHadDownedSize, nTotalNeedDownSize);
        }

        private void OnFileDownloadComplete(string url, bool bSuc, DownResInfo resInfo, DownResFile resFile)
        {
            string fileSavePath = null;
            if (bSuc)
            {
                if (resFile != null && resFile.file != null)
                    fileSavePath = resFile.file.Name;

                m_successDownList.Add(resFile);
            }
            else
            {
                m_failedDownList.Add(resFile);
            }

            m_downloadFinish?.Invoke(url, fileSavePath);
        }
    }

}
