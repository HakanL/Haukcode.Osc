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
		long totalAtPeriodStart = 0;

		public long Total { get; private set; }

		public float Rate { get; private set; }

		public void Increment(int amount)
		{
			Total += amount;
		}

		internal void Update(double ticks)
		{
			double countInPeriod = (double)(Total - totalAtPeriodStart);

			Rate = (float)(countInPeriod / ticks);

			totalAtPeriodStart = Total;
		}

		public void Reset()
		{
			Total = 0;
			Rate = 0;
			totalAtPeriodStart = 0;
		}
	}

	public class OscCommunicationStatistics : IDisposable
	{
		List<SingleStatistic> statistics = new List<SingleStatistic>();
		bool shouldStop = false; 
		Thread thread; 

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

			statistics.Add(BytesReceived);
			statistics.Add(BytesSent);

			statistics.Add(PacketsReceived);
			statistics.Add(PacketsSent);

			statistics.Add(MessagesReceived);
			statistics.Add(MessagesSent);

			statistics.Add(BundlesReceived);
			statistics.Add(BundlesSent);

			statistics.Add(ReceiveErrors); 
		}

		public void Start()
		{
			Stop();

			PeriodStart = DateTime.Now;

			shouldStop = false; 

			thread = new Thread(UpdateLoop);

			thread.Start(); 
		}

		public void Stop()
		{
			shouldStop = true; 

			if (thread != null)
			{
				thread.Join(); 
			}

			thread = null;
		}

		public void Reset()
		{
			PeriodStart = DateTime.Now;

			foreach (SingleStatistic stat in statistics)
			{
				stat.Reset(); 
			}
		}

		private void UpdateLoop()
		{
			while (shouldStop == false)
			{
				thread.Join(1000);

				Update(); 
			}
		}

		private void Update()
		{
			DateTime now = DateTime.Now;
			double ticks = (double)(now.Ticks - PeriodStart.Ticks);

			ticks /= (double)TimeSpan.TicksPerSecond;

			foreach (SingleStatistic stat in statistics)
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
