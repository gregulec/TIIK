using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TIIK
{
    public partial class MainPage : Page
    {
        List<Symbol> series = new List<Symbol>();
        string filetoCompres,filetoDecompres;
        public MainPage()
        {
            InitializeComponent();
        }

        private int findPosition(List<Symbol> sorted)
        {
            int pos = 0;
            double dif = 1;
            for (int i = 0; i < sorted.Count; i++)
            {
                double sum1 = 0, sum2 = 0, difference = 0;
                for (int j = 0; j <= i; j++)
                {
                    sum1 = sum1 + sorted[j].getProbility();
                }
                for (int k = i + 1; k < sorted.Count; k++)
                {
                    sum2 = sum2 + sorted[k].getProbility();
                }
                difference = Math.Abs(sum1 - sum2);
                if (difference < dif)
                {
                    dif = difference;
                    pos = i + 1;
                }
            }
            return pos;
        }

        private void ShannonaFano(List<Symbol> sorted)
        {
            if (sorted.Count == 2)
            {
                foreach (Symbol symbol in series)
                {
                    if (sorted[0] == symbol ) symbol.setCode('0');
                    if (sorted[1] == symbol ) symbol.setCode('1');
                }
            }
            if(sorted.Count > 2)
            {
                int pos = findPosition(sorted);
                List<Symbol> s1 = new List<Symbol>();
                List<Symbol> s2 = new List<Symbol>();
                s1 = sorted.GetRange(0, pos);
                s2 = sorted.GetRange(pos, sorted.Count - pos);
                foreach(Symbol symbol in series)
                {
                    if(s1.Contains(symbol)) symbol.setCode('0');
                    if(s2.Contains(symbol)) symbol.setCode('1');
                }
                ShannonaFano(s1);
                ShannonaFano(s2);
            }
        }

        public byte[] ToByteArray(BitArray bits)
        {
            byte[] bytes = new byte[bits.Length / 8];
            int j = 0;
            for (int i = 0; i < bits.Length; i+=8)
            {
                int oneByte=0;
                if (bits[i]) oneByte = oneByte + 128;
                if (bits[i+1]) oneByte = oneByte + 64;
                if (bits[i+2]) oneByte = oneByte + 32;
                if (bits[i+3]) oneByte = oneByte + 16;
                if (bits[i+4]) oneByte = oneByte + 8;
                if (bits[i+5]) oneByte = oneByte + 4;
                if (bits[i+6]) oneByte = oneByte + 2;
                if (bits[i+7]) oneByte = oneByte + 1;
                bytes[j] = (byte)oneByte;
                j++;
            }

            return bytes;
        }

        public static BitArray BitsReverse(BitArray bits)
        {
            int len = bits.Count;
            BitArray a = new BitArray(bits);
            BitArray b = new BitArray(bits);
            for (int i = 0, j = len - 1; i < len; ++i, --j)
            {
                a[i] = a[i] ^ b[j];
                b[j] = a[i] ^ b[j];
                a[i] = a[i] ^ b[j];
            }
            return a;
        }

        private void openfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text File | *.txt";
                dlg.ShowDialog();
                filetoCompres = System.IO.File.ReadAllText(dlg.FileName);
            }
            catch
            {
                MessageBox.Show("       Error");
            }
            for (int i = 0; i < 128; i++)
            {
                series.Add(new Symbol(i));
            }
            for (int i = 0; i < filetoCompres.Length; i++)
            {
                //series[filetoCompres[i]].setInstances();
                //double p = series[filetoCompres[i]].getInstances() / (double)filetoCompres.Length;
                //series[filetoCompres[i]].setProbility(p);
                Symbol temp = series.Find(symbol => symbol.getSymbol() == filetoCompres[i]);
                if(temp != null)
                {
                    temp.setInstances();
                    double p = temp.getInstances() / (double)filetoCompres.Length;
                    temp.setProbility(p);
                }
            }

            series.RemoveAll(symbol => symbol.getInstances() == 0);
            series = series.OrderByDescending(symbol => symbol.getProbility()).ToList();
            ShannonaFano(series);
            double entropy = 0;
            double averageKodeSize = 0;
            resultP.Text = "";
            resultC.Text = "";
            foreach (Symbol symbol in series)
            {
                resultP.Text = resultP.Text + "Znak \"" + (char)symbol.getSymbol() + "\" występuje z czętotliwością " + symbol.getProbility() + "\n";
                entropy = entropy + symbol.getProbility() * Math.Log((1.0 / symbol.getProbility()), 2.0);
                resultC.Text = resultC.Text +"Znak \"" + (char)symbol.getSymbol() + "\" ma kod " + symbol.getCode()+"\n";
                double kodeSize = symbol.getCodeLenght() * symbol.getProbility();
                averageKodeSize = averageKodeSize + kodeSize;
            }
            resultP.Text = resultP.Text + "Entropia wynosi: " + entropy + "\n";
            resultC.Text = resultC.Text + "Srednia długość kodu wynosi: " + averageKodeSize + "\n";
            int sizeBitArray = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < filetoCompres.Length; i++)
            {
                //var temp = series.Where(symbol => symbol.getSymbol() == filetoCompres[i]);
                //sb.Append(temp.First().getCode());
                //sizeBitArray = sizeBitArray + temp.First().getCodeLenght();
                Symbol temp = series.Find(symbol => symbol.getSymbol() == filetoCompres[i]);
                if (temp!= null)
                {
                    sb.Append(temp.getCode());
                    sizeBitArray = sizeBitArray + temp.getCodeLenght();
                }
            }
            switch (sizeBitArray%8)
            {
                case 1:
                    sizeBitArray = sizeBitArray + 7;
                    break;
                case 2:
                    sizeBitArray = sizeBitArray + 6;
                    break;
                case 3:
                    sizeBitArray = sizeBitArray + 5;
                    break;
                case 4:
                    sizeBitArray = sizeBitArray + 4;
                    break;
                case 5:
                    sizeBitArray = sizeBitArray + 3;
                    break;
                case 6:
                    sizeBitArray = sizeBitArray + 2;
                    break;
                case 7:
                    sizeBitArray = sizeBitArray + 1;
                    break;
                default:
                    break;
            }
            BitArray bitArray = new BitArray(sizeBitArray);
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == '0') bitArray[i] = false;
                else bitArray[i] = true;
            }
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "compressed";
                dlg.DefaultExt = ".bin";
                dlg.Filter = "Binary File (*.bin)|*.bin";
                if (dlg.ShowDialog() == true)
                { 
                    BinaryWriter bw = new BinaryWriter(dlg.OpenFile());
                    byte[] buffer = ToByteArray(bitArray);
                    bw.Write(buffer, 0, buffer.Length);
                    bw.Dispose();
                    bw.Close();
                    MessageBox.Show("       Done");
                }
            }
            catch
            {
                MessageBox.Show("       Error");
            }           
        }

        private void decode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".bin";
                dlg.FileName = "compressed";
                dlg.Filter = "Binary File (*.bin)|*.bin";
                dlg.ShowDialog();
                filetoDecompres = dlg.FileName;
            }
            catch
            {
                MessageBox.Show("       Error");
            }
  
            BinaryReader br = new BinaryReader(File.Open(filetoDecompres, FileMode.Open));
            byte[] buffor = br.ReadBytes((int)br.BaseStream.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(buffor);
            BitArray bitArray = new BitArray(buffor);
            bitArray = BitsReverse(bitArray);
            StringBuilder sb = new StringBuilder();
            StringBuilder decoded = new StringBuilder();

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i] == false) sb.Append('0');
                else sb.Append('1');
                Symbol temp = series.Find(symbol => symbol.getCode() == sb.ToString());
                if(temp != null)
                {
                    decoded.Append((char)temp.getSymbol());
                    sb.Clear();
                }
                //var temp = series.Where(symbol => symbol.getCode() == sb.ToString());
                //if (temp.Count() == 1)
                //{
                //    decoded.Append((char)temp.First().getSymbol());
                //    sb.Clear();
                //}
            }
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "decompresed";
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text File | *.txt";
                if (dlg.ShowDialog() == true)
                {
                    StreamWriter sw = new StreamWriter(dlg.OpenFile());
                    sw.Write(decoded);
                    MessageBox.Show("       Done");
                    sw.Dispose();
                    sw.Close();
                }
            }
            catch
            {
                MessageBox.Show("       Error");
            }
        }
    }
}
