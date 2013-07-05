using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rug.Osc;
using System.Net;
using System.Threading;

namespace OscTester
{
    public partial class DemoForm : Form
    {
        enum ArgType
        {
            Int = 0, 
            Float = 1, 
            String = 2,
            Blob = 3,
            Mix1 = 4
        }

        public DemoForm()
        {
            InitializeComponent();

            comboBox1.Items.Clear(); 
            comboBox1.Items.AddRange(Enum.GetNames(typeof(ArgType)));
            comboBox1.SelectedIndex = 0; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear(); 

            //OscSender oscSend = new OscSender(IPAddress.Parse("192.168.2.6"), 5000);
            OscSender oscSend = new OscSender(IPAddress.Broadcast, 5000);

            ArgType type = (ArgType)comboBox1.SelectedIndex;

			string messageAddress = textBox1.Text;

            try
            {
                richTextBox1.AppendText("Connect to " + oscSend.Address.ToString() + ":" + oscSend.Port.ToString() + "\n");
                oscSend.Connect();

                long timeBegin = DateTime.Now.Ticks;

                double averageSend = 0;
                double averageComplete = 0;

                double count = 256;
                int messageCount = 600;

                for (int j = 0; j < count; j++)
                {
                    long timeStart = DateTime.Now.Ticks;

                    richTextBox1.AppendText("Sending\n");

                    for (int i = 0; i < messageCount; i++)
                    {
                        OscMessage msg;

                        switch (type)
                        {
                            case ArgType.Int:
								msg = new OscMessage(messageAddress, (int)4242);
                                break;
                            case ArgType.Float:
								msg = new OscMessage(messageAddress, (float)4242);
                                break;
                            case ArgType.String:
								msg = new OscMessage(messageAddress, 4242.ToString());
                                break;
                            case ArgType.Blob:
								msg = new OscMessage(messageAddress, Encoding.UTF8.GetBytes(4242.ToString()));
                                break;
                            case ArgType.Mix1:
                                msg = new OscMessage(messageAddress, (int)123, (float)456); // , 789.ToString());
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        oscSend.Send(msg);
                    }

                    long timeSent = DateTime.Now.Ticks;

                    oscSend.WaitForAllMessagesToComplete();

                    long timeComplete = DateTime.Now.Ticks;

                    richTextBox1.AppendText("Done\n");

                    double sendMS = ((double)(timeSent - timeStart) / (double)TimeSpan.TicksPerMillisecond);
                    double completeMS = ((double)(timeComplete - timeStart) / (double)TimeSpan.TicksPerMillisecond);


                    richTextBox1.AppendText(sendMS.ToString("N2") + "ms to send\n");
                    richTextBox1.AppendText(completeMS.ToString("N2") + "ms to complete\n");

                    averageSend += sendMS;
                    averageComplete += completeMS; 

                    richTextBox1.AppendText("\n");
                }

                long timeEnd = DateTime.Now.Ticks;

                double totalTime = ((double)(timeEnd - timeBegin) / (double)TimeSpan.TicksPerSecond);

                richTextBox1.AppendText("\n");
                richTextBox1.AppendText("\n");

                richTextBox1.AppendText("Total Time: " + totalTime.ToString("N2") + "s\n");

                richTextBox1.AppendText((messageCount * count).ToString() + " messages sent\n");
                richTextBox1.AppendText("Av. " + (averageSend / count).ToString("N2") + "ms to send\n");
                richTextBox1.AppendText("Av. " + (averageComplete / count).ToString("N2") + "ms to complete\n");

            }
            catch (Exception ex)
            {
                richTextBox1.AppendText("Exception: " + ex.Message + "\n");
            }
            finally
            {
                oscSend.Dispose(); 
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear(); 

            OscReceiver oscReceiver = new OscReceiver(5000);

            try
            {
                richTextBox1.AppendText("Connect to " + oscReceiver.Address.ToString() + ":" + oscReceiver.Port.ToString() + "\n");
                oscReceiver.Connect();

                richTextBox1.AppendText("Receiving\n");

                for (int i = 0; i < 1000; i++)
                {
					OscMessage msg;

                    if (oscReceiver.TryReceive(out msg) == false)
                    {
                        Thread.CurrentThread.Join(100); 
                        continue; 
                    }

                    richTextBox1.AppendText("Message\n");
                    richTextBox1.AppendText(msg.Address + "\n");

                    richTextBox1.AppendText(msg.Arguments.Length.ToString() + " Args\n");

					for (int j = 0; j < msg.Arguments.Length; j++)
                    {
						richTextBox1.AppendText(msg.Arguments[j].ToString() + "\n");
                    }

                    richTextBox1.AppendText("\n");

                    Thread.CurrentThread.Join(100); 
                }

                richTextBox1.AppendText("Done\n");
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText("Exception: " + ex.Message + "\n");
            }
            finally
            {
                oscReceiver.Dispose();
            }
        }
    }
}
