using System;
using System.Collections.Generic;
using System.Drawing;

using System.Windows.Forms;
using NetManager;


namespace EMG
{
    public partial class Form1 : Form
    {
        List<String> titles = new List<String>();
        int[] touch_arr = new int[23];
        int accBucketsCount = 0;
        int analise_window = 0;
        int dec = 0;
        int pizza_1 = 0;
        int pizza_2 = 0;
        bool pizza_1_is_active = false;
        bool pizza_2_is_active = false;
        double[] minV = { 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999 };
        double[] maxV = { 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] hand_command = new int[5];
        int[] drone_command = new int[8];
        int[] bpla_command = new int[4];
        PictureBox[] pictureBoxes_touch;
        PictureBox[] pictureBoxes_finger;
        PictureBox[] pictureBoxes_drone;
        PictureBox[] pictureBoxes_pizza_1;
        PictureBox[] pictureBoxes_pizza_2;
        PictureBox[] pictureBoxes_bpla_pizza_1;
        PictureBox[] pictureBoxes_bpla_pizza_2;
        CheckBox[] checkBoxes_drone;
        ProgressBar[] progressBars;
        TrackBar[] trackBars;
        bool tresholdFlag = false;
        bool timerFlag = false;
        double[] currentAbsValue = new double[29];
        Dictionary<String, List<double>> meanDictionary = new Dictionary<String, List<double>>();
        Dictionary<String, List<double>> currentDictionary = new Dictionary<String, List<double>>();

        public Form1()
        {
            
            for (int i = 0; i < touch_arr.Length; i++)
                touch_arr[i] = 0;
            for (int i = 0; i < hand_command.Length; i++)
                hand_command[i] = 0;
            
            InitializeComponent();
            pictureBoxes_touch = new PictureBox[] {pictureBoxtl, pictureBoxtm, pictureBoxtr, pictureBoxml, pictureBoxc, pictureBoxmr, pictureBoxll,
            pictureBoxlm, pictureBoxlr, pictureBoxtp, pictureBoxmp, pictureBoxlp, pictureBoxrt, pictureBoxrm, pictureBoxrl, pictureBoxum,
            pictureBoxmm, pictureBoxlfm, pictureBoxti, pictureBoxmi, pictureBoxli, pictureBoxtt, pictureBoxlt};
            pictureBoxes_finger = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5 };
            pictureBoxes_drone = new PictureBox[] { pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12, pictureBox13 };
            checkBoxes_drone = new CheckBox[] { checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8, checkBox9};
            pictureBoxes_pizza_1 = new PictureBox[] { pizza_1_1, pizza_1_2, pizza_1_3, pizza_1_4 };
            pictureBoxes_pizza_2 = new PictureBox[] { pizza_2_1, pizza_2_2, pizza_2_3, pizza_2_4 };
            pictureBoxes_bpla_pizza_1 = new PictureBox[] { bpla_pizza_1_1, bpla_pizza_1_2 };
            pictureBoxes_bpla_pizza_2 = new PictureBox[] { bpla_pizza_2_1, bpla_pizza_2_2 };
            progressBars = new ProgressBar[] { progressBar3, progressBar2 };
            trackBars = new TrackBar[] { trackBar2, trackBar1 };

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            titles.Add("FP1");
            titles.Add("F3");
            titles.Add("C3");
            titles.Add("P3");
            titles.Add("O1");
            titles.Add("F7");
            titles.Add("T3");
            titles.Add("T5");
            titles.Add("FZ");
            titles.Add("PZ");
            titles.Add("A1-A2");
            titles.Add("FP2");
            titles.Add("F4");
            titles.Add("C4");
            titles.Add("P4");
            titles.Add("O2");
            titles.Add("F8");
            titles.Add("T4");
            titles.Add("T6");
            titles.Add("FPZ");
            titles.Add("CZ");
            titles.Add("OZ");
            titles.Add("E1");
            titles.Add("E2");
            titles.Add("E3");
            titles.Add("E4");
            titles.Add("Не используется");
            titles.Add("Дыхание");
            titles.Add("Служебный канал");

            for (int i = 0; i < 29; i++)
            {
                meanDictionary.Add(titles[i], new List<double>());
            }

            clientControl1.Client.StartClient();
            reseiveClientControl1.Client.StartClient();
        }

        private void MessageHandler(object sender, NetManager.EventClientMsgArgs e)
        {
            if (control_start_button.InvokeRequired)
            {
                control_start_button.Invoke(new Action(() =>
                {
                    // Код для обновления элемента управления
                    control_start_button.Enabled = true;
                }));
            }
            else
            {
                control_start_button.Enabled = true;
            }
            Frame f = new Frame(e.Msg);
            var bucket = new double[29, 24];
            var data = f.Data;
            if (timerFlag)
            {

                accBucketsCount += 24;

                for (int i = 0; i < 29; i++)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        bucket[i, j] = Convert.ToDouble(data[i * 24 + j]);
                        if (meanDictionary[titles[i]].Count > currentDictionary[titles[i]].Count)
                            currentDictionary[titles[i]].Add(bucket[i, j]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 29; i++)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        bucket[i, j] = Convert.ToDouble(data[i * 24 + j]);
                    }
                }
            }

            if (!timerFlag && timer1.Enabled)
            {
                for (int i = 0; i < 29; i++)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        meanDictionary[titles[i]].Add(bucket[i, j]);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        { 
                dec = Convert.ToInt32(decimation_textBox.Text);
                analise_window = Convert.ToInt32(analize_woindow_textBox.Text);
                timer1.Interval = analise_window;
                timer1.Start();
        }

        private void control(int i)
        {
            if (drone_radioButton.Checked)
            {
                drone_control(i);
            }
            else if(bpla_radioButton.Checked)
            {
                bpla_control();
            }
            else
            {
                hand_control(i);
            }
        }

        private void change_pizza_flag()
        {
            if (pizza_control_without_buttons_checkBox.Checked)
            {

                pizza_1_is_active = (currentAbsValue[0] > (maxV[0] - minV[0]) / (Convert.ToDouble(trackBars[0].Maximum) / Convert.ToDouble(trackBars[0].Value)) + minV[0]) ? true : false;

                pizza_2_is_active = (currentAbsValue[11] > (maxV[1] - minV[1]) / (Convert.ToDouble(trackBars[0].Maximum) / Convert.ToDouble(trackBars[1].Value)) + minV[1]) ? true : false;
            }
        }

        private void drone_control(int i)
        {

                if (pizza_control_checkBox.Checked)
                {
                    for (int j = 0; j < drone_command.Length; j++)
                    {
                        drone_command[j] = 0;
                }
                    change_pizza_flag();
                if (!proportional_control_checkBox.Checked)
                    disc_control();
                else propor_control();
                }

                else
                {
                    if (checkBoxes_drone[i].Checked)
                    {
                        drone_command[i] = (currentAbsValue[i] > (maxV[i] - minV[i]) / 2 + minV[i]) ? 1 : 0;

                        pictureBoxes_drone[i].BackColor = (currentAbsValue[i] > (maxV[i] - minV[i]) / 2 + minV[i]) ? Color.Green : Color.Red;
                    }
                }

        }

        private void propor_control()
        {
            if (pizza_1_is_active)
            {
                try
                {
                    double tresh = (Convert.ToDouble(trackBars[0].Value) / Convert.ToDouble(trackBars[0].Maximum) * (maxV[0] - minV[0]) + minV[0]);
                    int speed = Convert.ToInt32(50 * (Math.Abs(currentAbsValue[0] - tresh) / maxV[0]));
                    drone_command[pizza_1] = speed > 50 ? 50 : speed;

                    pictureBox14.Visible = true;
                }
                catch(Exception ex)
                {

                }
            }
            else
            {
                pictureBox14.Visible = false;

            }
            if (pizza_2_is_active)
            {
                try { 
                double tresh = (Convert.ToDouble(trackBars[1].Value) / Convert.ToDouble(trackBars[1].Maximum) * (maxV[1] - minV[1]) + minV[1]);
                
                 int speed = Convert.ToInt32(50 * (Math.Abs(currentAbsValue[1] - tresh) / maxV[1]));
                drone_command[pizza_2 + 4] = speed > 50 ? 50 : speed;
                pictureBox15.Visible = true;
            }
                catch (Exception ex)
            {

            }
        }
            else pictureBox15.Visible = false;
        }

        private void disc_control()
        {
            if (pizza_1_is_active)
            {
                drone_command[pizza_1] = 1;
                pictureBox14.Visible = true;
            }
            else
            {
                pictureBox14.Visible = false;

            }
            if (pizza_2_is_active)
            {
                drone_command[pizza_2 + 4] = 1;
                pictureBox15.Visible = true;
            }
            else pictureBox15.Visible = false;
        }
        private void bpla_control()
        {
            if (pizza_control_checkBox.Checked)
            {
                for (int j = 0; j < bpla_command.Length; j++)
                {
                    bpla_command[j] = 0;
                }
                change_pizza_flag();
                if (pizza_1_is_active)
                {
                    bpla_command[pizza_1] = 1;
                    pictureBox21.Visible = true;
                }
                else
                    pictureBox21.Visible = false;
                if (pizza_2_is_active)
                {
                    bpla_command[pizza_2 + 2] = 1;
                    pictureBox22.Visible = true;
                }
                else
                    pictureBox22.Visible = false;
            }
        }

        private void hand_control(int i)
        {
            if (currentAbsValue[i] > (maxV[i] - minV[i]) / trackBarUp.Value + minV[i])
            {
                pictureBoxes_finger[i].BackColor = Color.Green;
                hand_command[i] = 1;
            }
            else if ((currentAbsValue[i] > (maxV[i] - minV[i]) / trackBarDown.Value + minV[i]) && (currentAbsValue[i] < (maxV[i] - minV[i]) / trackBarUp.Value + minV[i]))
            {
                pictureBoxes_finger[i].BackColor = Color.Yellow;
                hand_command[i] = 2;
            }
            else
            {
                pictureBoxes_finger[i].BackColor = Color.Red;
                hand_command[i] = 0;
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!timerFlag)
            {
                for (int i = 0; i < 29; i++)
                {
                    currentDictionary.Add(titles[i], new List<double>());
                }
                timerFlag = true;
            }
            else
            {
                
                //try
                //{
                    if (drone_radioButton.Checked)
                    {
                        if (pizza_control_checkBox.Checked)
                        {
                            for (int i = 0; i < 2; i++)
                                if (meanDictionary[titles[i*11]].Count == currentDictionary[titles[i*11]].Count)
                                {
                                    var array = Abs_value.calculate_abs(currentDictionary[titles[i*11]].ToArray(), analise_window / 10, dec);
                                    double summ = 0;
                                    for (int j = 0; j < array.Length; j++)
                                        summ += array[j];
                                    double mid = summ / array.Length;
                                    currentAbsValue[i*11] = mid;
                                    if (tresholdFlag)
                                    {
                                        if (mid < minV[i])
                                            minV[i] = mid;
                                        if (mid > maxV[i])
                                            maxV[i] = mid;
                                    }
                                    currentDictionary[titles[i*11]].Clear();
                                try
                                {
                                    progressBars[i].Minimum = Convert.ToInt32(minV[i]);
                                    progressBars[i].Maximum = Convert.ToInt32(maxV[i]);
                                    progressBars[i].Value = Convert.ToInt32(mid);
                                }
                                catch(Exception ex)
                                {

                                }
                                    control(i);
                                }
                        }
                        else
                        {
                            for (int i = 0; i < 8; i++)
                                if (meanDictionary[titles[i]].Count == currentDictionary[titles[i]].Count)
                                {

                                    var array = Abs_value.calculate_abs(currentDictionary[titles[i]].ToArray(), analise_window / 10, dec);
                                    double summ = 0;
                                    for (int j = 0; j < array.Length; j++)
                                        summ += array[j];
                                    double mid = summ / array.Length;
                                    currentAbsValue[i] = mid;
                                    if (tresholdFlag)
                                    {
                                        if (mid < minV[i])
                                            minV[i] = mid;
                                        if (mid > maxV[i])
                                            maxV[i] = mid;
                                    }

                                    currentDictionary[titles[i]].Clear();
                                    control(i);
                                }
                        }
                    }
                    if(bpla_radioButton.Checked)
                    {
                        if (pizza_control_without_buttons_checkBox.Checked)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if (meanDictionary[titles[i * 11]].Count == currentDictionary[titles[i * 11]].Count)
                                {
                                    var array = Abs_value.calculate_abs(currentDictionary[titles[i * 11]].ToArray(), analise_window / 10, dec);
                                    double summ = 0;
                                    for (int j = 0; j < array.Length; j++)
                                        summ += array[j];
                                    double mid = summ / array.Length;
                                    currentAbsValue[i * 11] = mid;
                                    if (tresholdFlag)
                                    {
                                        if (mid < minV[i])
                                            minV[i] = mid;
                                        if (mid > maxV[i])
                                            maxV[i] = mid;
                                    }

                                    currentDictionary[titles[i * 11]].Clear();
                                    progressBars[i].Minimum = Convert.ToInt32(minV[i]);
                                    progressBars[i].Maximum = Convert.ToInt32(maxV[i]);
                                    progressBars[i].Value = Convert.ToInt32(mid);
                                    control(i);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if (meanDictionary[titles[i]].Count == currentDictionary[titles[i]].Count)
                                {

                                    var array = Abs_value.calculate_abs(currentDictionary[titles[i]].ToArray(), analise_window / 10, dec);
                                    double summ = 0;
                                    for (int j = 0; j < array.Length; j++)
                                        summ += array[j];
                                    double mid = summ / array.Length;
                                    currentAbsValue[i] = mid;
                                    if (tresholdFlag)
                                    {
                                        if (mid < minV[i])
                                            minV[i] = mid;
                                        if (mid > maxV[i])
                                            maxV[i] = mid;
                                    }

                                    currentDictionary[titles[i]].Clear();
                                    control(i);

                                }
                            }
                        }

                    }
                    else
                    {


                        for (int i = 0; i < 5; i++)
                            if (meanDictionary[titles[i]].Count == currentDictionary[titles[i]].Count)
                            {

                                var array = Abs_value.calculate_abs(currentDictionary[titles[i]].ToArray(), analise_window / 10, dec);
                                double summ = 0;
                                for (int j = 0; j < array.Length; j++)
                                    summ += array[j];
                                double mid = summ / array.Length;
                                currentAbsValue[i] = mid;
                                if (tresholdFlag)
                                {
                                    if (mid < minV[i])
                                        minV[i] = mid;
                                    if (mid > maxV[i])
                                        maxV[i] = mid;
                                }

                                currentDictionary[titles[i]].Clear();
                                control(i);
                            }
                    }
                //}
                //catch(Exception ex)
                //{
                //    timer1.Stop();
                //    MessageBox.Show("Необходим сигнал", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);                  
                //}

            }
        }
        
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (drone_radioButton.Checked)
            {
                if (!pizza_1_is_active)
                {
                    for (int i = 0; i < pictureBoxes_pizza_1.Length; i++)
                    {
                        if (i == pizza_1)
                            pictureBoxes_pizza_1[i].Visible = true;
                        else
                            pictureBoxes_pizza_1[i].Visible = false;
                    }

                    pizza_1 = (pizza_1 + 1) % 4;
                }
            }
            if (bpla_radioButton.Checked)
            {
                if (!pizza_1_is_active)
                {
                    for (int i = 0; i < pictureBoxes_bpla_pizza_2.Length; i++)
                    {
                        if (i == pizza_1)
                            pictureBoxes_bpla_pizza_1[i].Visible = true;
                        else
                            pictureBoxes_bpla_pizza_1[i].Visible = false;
                    }
                    pizza_1 = (pizza_1 + 1) % 2;
                }
            }


        }
        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            hand_command[0] = 0;
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            hand_command[0] = 0;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {

                if (drone_radioButton.Checked)
                {
                    byte[] result = new byte[drone_command.Length * sizeof(int)];

                    Buffer.BlockCopy(drone_command, 0, result, 0, result.Length);

                    clientControl1.Client.SendData(clientControl1.CheckedClientAddresses[0], result);

                }
                else if (bpla_radioButton.Checked)
                {
                    byte[] result = new byte[bpla_command.Length * sizeof(int)];

                    Buffer.BlockCopy(bpla_command, 0, result, 0, result.Length);

                    clientControl1.Client.SendData(clientControl1.CheckedClientAddresses[0], result);

                }
                else
                {
                    byte[] result = new byte[hand_command.Length * sizeof(int)];

                    Buffer.BlockCopy(hand_command, 0, result, 0, result.Length);

                    clientControl1.Client.SendData(clientControl1.CheckedClientAddresses[0], result);

                }
            }
            catch (Exception ex) {
                timer3.Stop();
                MessageBox.Show("Сперва запустите управление", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer3.Start();
        }

        private void reseiveClientControl1_ReseiveData(object sender, EventClientMsgArgs e)
        {
            for (int i = 0; i < (touch_arr.Length); i++)
            {
                touch_arr[i] = BitConverter.ToInt32(e.Msg, i * sizeof(int));
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < touch_arr.Length; i++)
            {
                if (touch_arr[i] == 1)
                    pictureBoxes_touch[i].Visible = true;
                else
                    pictureBoxes_touch[i].Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer4.Start();
        }

        private void trackBarUp_Scroll(object sender, EventArgs e)
        {
            if(trackBarDown.Value< trackBarUp.Value)
            {
                trackBarDown.Value = trackBarUp.Value;
            }
        }

        private void trackBarDown_Scroll(object sender, EventArgs e)
        {
            if (trackBarUp.Value > trackBarDown.Value)
            {
                trackBarUp.Value = trackBarDown.Value;
            }
        }

        private void clientControl1_Load(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!tresholdFlag)
            {
                for (int i = 0; i < maxV.Length; i++)
                {
                    maxV[i] = 0;
                    minV[i] = 99999999;
                }
            }
            tresholdFlag = !tresholdFlag;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bpla_radioButton.Checked = false;
            groupBox1.Visible = true;
            groupBox3.Visible = true;
            groupBox4.Visible = false;
            groupBox5.Visible = false;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer2.Start();          
            timer5.Start();
            timer6.Start();
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            if (drone_radioButton.Checked)
            {
                if (!pizza_2_is_active)
                {
                    for (int i = 0; i < pictureBoxes_pizza_2.Length; i++)
                    {
                        if (i == pizza_2)
                            pictureBoxes_pizza_2[i].Visible = true;
                        else
                            pictureBoxes_pizza_2[i].Visible = false;
                    }
                    pizza_2 = (pizza_2 + 1) % 4;
                }
            }
            if(bpla_radioButton.Checked)
            {
                if (!pizza_2_is_active)
                {
                    for (int i = 0; i < pictureBoxes_bpla_pizza_2.Length; i++)
                    {
                        if (i == pizza_2)
                            pictureBoxes_bpla_pizza_2[i].Visible = true;
                        else
                            pictureBoxes_bpla_pizza_2[i].Visible = false;
                    }
                    pizza_2 = (pizza_2 + 1) % 2;
                }
            }

        }

        private void pizza_1_2_MouseDown(object sender, MouseEventArgs e)
        {
           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pizza_1_is_active = !pizza_1_is_active;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            pizza_2_is_active = !pizza_2_is_active;
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {

        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            if (drone_radioButton.Checked)
            {
                if (pizza_1_is_active)
                {
                    pictureBox14.Visible = true;
                }
                else
                {
                    pictureBox14.Visible = false;

                }
                {
                    if (pizza_2_is_active)
                    {
                        pictureBox15.Visible = true;
                    }
                    else pictureBox15.Visible = false;
                }
            }
            if (bpla_radioButton.Checked)
            {
                if (pizza_1_is_active)
                {
                    pictureBox21.Visible = true;
                }
                else
                {
                    pictureBox21.Visible = false;

                }
                {
                    if (pizza_2_is_active)
                    {
                        pictureBox22.Visible = true;
                    }
                    else pictureBox22.Visible = false;
                }
            }
        }



        private void pictureBox18_Click(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

            groupBox1.Visible = true;
            groupBox3.Visible = true;
            groupBox4.Visible = false;
            groupBox5.Visible = false;
            drone_groupBox.Visible = true;
            hand_groupBox.Visible = false;
            channels_groupBox.Visible = false;
        }

        private void hand_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            groupBox3.Visible = false;
            groupBox4.Visible = false;
            groupBox5.Visible = false;
            drone_groupBox.Visible = false;
            hand_groupBox.Visible = true;
            channels_groupBox.Visible = false;
        }

        private void bpla_radioButton_CheckedChanged(object sender, EventArgs e)
        {

            groupBox1.Visible = false;
            groupBox3.Visible = false;
            groupBox4.Visible = true;
            groupBox5.Visible = true;
            drone_groupBox.Visible = true;
            hand_groupBox.Visible = false;
            channels_groupBox.Visible = false;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            groupBox3.Visible = false;
            groupBox4.Visible = false;
            groupBox5.Visible = false;
            drone_groupBox.Visible = false;
            hand_groupBox.Visible = false;
            channels_groupBox.Visible = true;
        }
    }
}
