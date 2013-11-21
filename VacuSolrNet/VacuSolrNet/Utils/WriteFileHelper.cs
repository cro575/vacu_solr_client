// Copyright (C) 2002 Mark Pasternak
//
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
//
// Mark Pasternak (mark.pasternak@universum.se)


// 데브피아 ASP.NET 자료실에서 퍼온 파일 다운로드 클레스입니다.
// 요청하신 분이 계셔서 간단한 주석을 달았습니다.
//
// 주석 첨부 : 조성진 (http://powerof.net)


using System;
using System.Web;
using System.IO;

namespace VacuSolrNet
{
	/// <summary>
	/// The HttpResponse.WriteFile method does not work very well with large files since it reads
	/// the whole file to memory at once. This may crash the ASP.NET worker process if the file is too big.
	/// This helper class uses buffers and continuously checks if the client is connected before it sends any output.
	///
	/// HttpResponse.WriteFile 메소드는 아주 큰 파일을 전송하는 경우 잘 동작하지 않습니다. 
	/// 왜냐면 이 메소드는 전송할 파일을 메모리로 한번에 모두 읽어드리기 때문에 
	/// 전송할 파일이 크다면 서버에 많은 부담을 줍니다. 심각한 경우는 ASP.NET 프로세스가 죽는 경우도 생기죠.
	/// 다음의 클레스는 버퍼를 이용해 파일을 읽고 전송하기 때문에 
	/// 기존 HttpResponse.WriteFile 메소드의 문제를 해결해줍니다.
	/// 그리고 클라이언트와의 연결을 계속 점검하기때문에 클라이언트에서 파일전송을 
	/// 취소하거나 다른 이유로 연결이 끊길경우 더이상 파일을 전송하지 않습니다.
	/// </summary>
	public class WriteFileHelper
	{
		private int m_bufferSize=4096; //버퍼크기를 4메가로 지정
		private HttpContext Context;
		public EventHandler DownloadCancelled;
		public EventHandler DownloadCompleted;

		public WriteFileHelper()
		{
			Context = HttpContext.Current;
		}

		/// <summary>
		/// Sets and gets the size of the buffer that is used when a file is read to memory.
		/// A larger buffer will require more memory, but will on the other hand make 
		/// it less resource intensive to send a file. Experiment to find a good balance.
		/// </summary>
		/// 위에서 m_bufferSize가 4096으로 기본적으로 설정되지만
		/// 원한다면 이 속성을 이용해 버퍼크기를 변경할수 있다.
		public int BufferSize
		{
			set{m_bufferSize=value;}
			get{return m_bufferSize;}
		}

		// 사용자자 파일 전송을 취소하거나 어떤 이유로 클라이언트와 연결이 끈기면 
		// 이 메소드가 호출됩니다.
		// 이 클레스 외부에서 DownloadCancelled 의 이벤트 헨들러를 지정하면
		// 이 메소드가 호출될때 임의의 처리를 할수 있습니다.
		protected void OnDownloadCancelled() 
		{ 
			if (DownloadCancelled != null) // 헨들러가 지정되 있다면
				DownloadCancelled(null, null); 
		}

		// 파일전송이 완료되면 이 메소드가 호출됩니다.
		// 이 클레스 외부에서 DownloadCompleted 의 이벤트 헨들러를 지정하면
		// 이 메소드가 호출될때 임의의 처리를 할수 있습니다.
		protected void OnDownloadCompleted() 
		{ 
			if (DownloadCompleted != null) // 헨들러가 지정되 있다면
				DownloadCompleted(null, null); 
		}

		/// <summary>
		/// Writes a file to the Response Stream
		/// </summary>
		/// <param name="filePath">A Path to a file</param>
		/// 전송할 파일의 경로를 인자로 이 메소드를 호출하면 
		/// 클라이언트로 파일을 보냅니다.
		/// 그러나 이 메소드대신 다음에 나오는 
		/// WriteFileToResponseStreamWithForceDownloadHeaders 메소드를 사용하는게 더 편리합니다.
		/// 그런 이유로 이 메소드는 별로 사용할 일이 없을듯 하군요
		public void WriteFileToResponseStream(string filePath)
		{
			if(!File.Exists(filePath)) // 요청한 파일이 없으면 에러를 일으킨다.
				throw new Exception("File Path does not exist : " + filePath);

			// 클라이언트로 파일을 전송한다.
			WriteBinaryFile_Internal(filePath);
		}

		/// <summary>
		/// Writes a file to the Response Stream and adds the headers s장the "Save As" dialog is presented to the user
		/// </summary>
		/// <param name="filePath">A Path to a file</param>
		/// 전송할 파일의 경로를 인자로 이 메소드를 호출하면 
		/// 클라이언트로 HTTP 헤더를 첨부해서 파일을 전송합니다.
		/// 첨부한 헤더로 인해 클라이언트에는 '열기','저장', '취소' 버튼이 있는 
		/// 대화상자가 열립니다.
		public void WriteFileToResponseStreamWithForceDownloadHeaders(string filePath)
		{
			if(!File.Exists(filePath)) // 요청한 파일이 없으면 에러를 일으킨다.
				throw new Exception("File Path does not exist : " + filePath);
            
			//Context.Response.Clear();
			// 해더를 추가한다
			//Context.Response.ContentType = "application/octet-stream";
			//Context.Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
			Context.Response.AddHeader("Content-Length", new FileInfo(filePath).Length.ToString());


			// 클라이언트로 파일을 전송한다.
			WriteBinaryFile_Internal(filePath);
		}

		// 실질적인 파일전송을 담당하는 메소드
		private void WriteBinaryFile_Internal(string filePath)
		{
			Context.Response.Buffer=false; // 버퍼링을 하지 않는다.
			FileStream inStr = null;
			byte[] buffer = new byte[m_bufferSize]; // 파일을 읽어드릴 버퍼
			long byteCount;
			try
			{
				inStr = File.OpenRead(filePath); // 파일을 연다.
				// m_bufferSize 크기 만큼씩 파일 내용을 차례대로 읽어들인다.
				while ((byteCount = inStr.Read(buffer, 0, buffer.Length)) > 0) 
				{
					// 클라이언트가 연결되 있는지 검사한다.
					// 즉, 사용자가 파일전송을 중간에 취소하거나
					// 다른 이유로 연결이 끈기면 IsClientConnected가 talse가 된다.
					if(Context.Response.IsClientConnected)
					{
						// 버퍼에 담긴 데이터를 클라이언트로 전송한다.
                        Context.Response.OutputStream.Write(buffer, 0, (int)byteCount);
						Context.Response.Flush();
					}
					else 
					{
						// 전송이 취소 됐음을 알리는 이벤트를 발생시킨다.
						OnDownloadCancelled();
						return;
					}
				}
				OnDownloadCompleted(); // 전송 완료 이벤트를 발생시킨다.
			}
			catch(Exception ex)
			{
				throw ex;
			}
			finally
			{
				inStr.Close();
				Context.Response.End();
			}
		}


	}
}
