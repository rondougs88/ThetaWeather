using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ThetaWeather
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader oReader = new StreamReader("..\\..\\Files\\weather.txt", Encoding.UTF8);
            string inputLine = "";
            string sPreContent = "";

            bool bAppendLine = false;
            //RL: Loop through each line of the txt file
            while ((inputLine = oReader.ReadLine()) != null)
            {
                if (bAppendLine && !inputLine.Trim().StartsWith("</"))
                {
                    sPreContent += inputLine + "\n";
                }

                if (inputLine.Trim().StartsWith("<"))
                {
                    
                    if (bAppendLine)    //RL: Condition is true when we get to the closing tag
                    {
                        bAppendLine = false;
                    }
                    else //RL: Condition is true when we get to the opening tag
                    {
                        bAppendLine = true;
                        //sPreContent += inputLine + "\n";
                    }
                    
                }

            }



            Console.Write(sPreContent);
            Console.ReadLine();
        }
    }
}
