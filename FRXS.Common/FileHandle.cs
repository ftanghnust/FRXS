/*****************************
* Author:CR
*
* Date:2015-11-20
******************************/

using System;


using System.IO;
using System.Net;
using System.Web;


namespace FRXS.Common.IO
{
    /// <summary>
    /// 文件处理
    /// </summary>
    public class FileHandle
    {
        /// <summary>
        /// 保存文件至服务器
        /// </summary>
        /// <param name="directory">服务器端文件目录</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="file">上传文件对象</param>
        /// <returns>服务器文件路径</returns>
        public static string SaveFileToServer(string directory, string fileName, HttpPostedFileBase file)
        {
            string serverPath = string.Empty;

            if (!string.IsNullOrWhiteSpace(directory) && !string.IsNullOrWhiteSpace(fileName))
            {
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    serverPath = string.Format(@"{0}\{1}", directory, fileName);
                    file.SaveAs(serverPath);
                }
                catch (Exception ex)
                {
                    //Logger.GetInstance().ExceptionLog(
                    //    new NormalLog
                    //    {
                    //        LogTime = DateTime.Now,
                    //        LogSource = "FileHandle/SaveFileToServer",
                    //        LogContent = ex.Message
                    //    }
                    //    );
                }
            }

            return serverPath;
        }

        /// <summary>
        /// 从url读取文件至文件流
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>文件流</returns>
        public static Stream ReadFileByUrl(string url)
        {
            Stream s = null;

            try
            {
                //无法获取所有url正则校验，所以去掉校验
                //if (RegularValidateHelper.UrlValidate(url))
                //{
                WebRequest wRequest = WebRequest.Create(url);
                WebResponse response = null;
                if (wRequest is FileWebRequest)
                {
                    response = (wRequest as FileWebRequest).GetResponse();
                }
                else if (wRequest is HttpWebRequest)
                {
                    response = (wRequest as HttpWebRequest).GetResponse();
                }
                if (response != null)
                {
                    s = response.GetResponseStream();
                }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                
            }

            return s;
        }

        /// <summary>
        /// 从url读取二进制文件填充字节数组
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="count">字节数</param>
        /// <returns>字节数组</returns>
        public static byte[] ReadFileByUrl(string url, int count)
        {
            byte[] b = null;
            Stream s = null;
            BinaryReader br = null;

            try
            {
                //无法获取所有url正则校验，所以去掉校验
                //if (RegularValidateHelper.UrlValidate(url))
                //{
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                s = response.GetResponseStream();
                br = new BinaryReader(s);

                b = br.ReadBytes(count);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                    s.Dispose();
                }
                if (br != null)
                {
                    br.Close();
                    br.Dispose();
                }
            }

            return b;
        }

        /// <summary>
        /// 保存字节数组至二进制文件
        /// </summary>
        /// <param name="byteArray">字节数组</param>
        /// <param name="directory">文件保存目录</param>
        /// <param name="fileName">文件保存名称</param>
        /// <returns>文件路径</returns>
        public static string SaveBinaryFileByByteArray(byte[] byteArray, string directory, string fileName)
        {
            string serverPath = string.Empty;

            FileStream fs = null;
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                serverPath = string.Format(@"{0}\{1}", directory, fileName);
                using (fs = new FileStream(serverPath, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(byteArray);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

            return serverPath;
        }

        /// <summary>
        /// 保存二进制文件至服务器
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="count">字节数</param>
        /// <param name="directory">文件保存目录</param>
        /// <param name="fileName">文件保存名称</param>
        /// <returns>文件路径</returns>
        public static string SaveBinaryFile(string url, int count, string directory, string fileName)
        {
            string serverPath = string.Empty;

            try
            {
                byte[] b = ReadFileByUrl(url, count);
                if (b != null && b.Length > 0)
                {
                    serverPath = SaveBinaryFileByByteArray(b, directory, fileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return serverPath;
        }
    }
}
