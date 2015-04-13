using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            String result;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("output.txt", true))
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                for (int m = 0; m < 2; m++)
                                {
                                    for (int n = 0; n < 2; n++)
                                    {
                                        for (int o = 0; o < 2; o++)
                                        {
                                            for (int p = 0; p < 2; p++)
                                            {
                                                result = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", i, j, k, l, m, n, o, p);
                                                file.WriteLine(result);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }          
            }
        }
    }
}
