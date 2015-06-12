/* 
 * Rug.Osc 
 * 
 * Copyright (C) 2013 Phill Tew (peatew@gmail.com)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 * 
 */

using System;
using System.Collections.Generic;
using System.Threading;

namespace Rug.Osc
{
	public class SingleStatistic
	{
		private long m_TotalAtPeriodStart = 0;

		public long Total { get; private set; }

		public float Rate { get; private set; }

		public void Increment(int amount)
		{
			Total += amount;
		}

		internal void Update(double ticks)
		{
			double countInPeriod = (double)(Total - m_TotalAtPeriodStart);

			Rate = (float)(countInPeriod / ticks);

			m_TotalAtPeriodStart = Total;
		}

		public void Reset()
		{
			Total = 0;
			Rate = 0;
			m_TotalAtPeriodStart = 0;
		}
	}

	public class OscCommunicationStatistics : IDisposable
	{
		private List<SingleStatistic> m_Statistics = new List<SingleStatistic>();
		private bool m_ShouldStop = false; 
		private Thread m_Thread; 

		public DateTime PeriodStart { get; private set; }

		public readonly SingleStatistic BytesReceived = new SingleStatistic();
		public readonly SingleStatistic BytesSent = new SingleStatistic();

		public readonly SingleStatistic PacketsReceived = new SingleStatistic();
		public readonly SingleStatistic PacketsSent = new SingleStatistic();

		public readonly SingleStatistic MessagesReceived = new SingleStatistic();
		public readonly SingleStatistic MessagesSent = new SingleStatistic();
		
		public readonly SingleStatistic BundlesReceived = new SingleStatistic();
		public readonly SingleStatistic BundlesSent = new SingleStatistic();

		public readonly SingleStatistic ReceiveErrors = new SingleStatistic();

		public OscCommunicationStatistics()
		{
			PeriodStart = DateTime.Now; 

			m_Statistics.Add(BytesReceived);
			m_Statistics.Add(BytesSent);

			m_Statistics.Add(PacketsReceived);
			m_Statistics.Add(PacketsSent);

			m_Statistics.Add(MessagesReceived);
			m_Statistics.Add(MessagesSent);

			m_Statistics.Add(BundlesReceived);
			m_Statistics.Add(BundlesSent);

			m_Statistics.Add(ReceiveErrors); 
		}

		public void Start()
		{
			Stop();

			PeriodStart = DateTime.Now;

			m_ShouldStop = false; 

			m_Thread = new Thread(UpdateLoop);

			m_Thread.Start(); 
		}

		public void Stop()
		{
			m_ShouldStop = true; 

			if (m_Thread != null)
			{
				m_Thread.Join(); 
			}

			m_Thread = null;
		}

		public void Reset()
		{
			PeriodStart = DateTime.Now;

			foreach (SingleStatistic stat in m_Statistics)
			{
				stat.Reset(); 
			}
		}

		private void UpdateLoop()
		{
			while (m_ShouldStop == false)
			{
				m_Thread.Join(1000);

				Update(); 
			}
		}

		private void Update()
		{
			DateTime now = DateTime.Now;
			double ticks = (double)(now.Ticks - PeriodStart.Ticks);

			ticks /= (double)TimeSpan.TicksPerSecond;

			foreach (SingleStatistic stat in m_Statistics)
			{
				stat.Update(ticks);
			}

			PeriodStart = now;			
		}

		public void Dispose()
		{
			Stop();
		}
	}
}
