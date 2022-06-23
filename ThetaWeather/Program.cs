using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;


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
            DataTable dtWeather = new DataTable();
            int lTableColumnsTxtIndex = -1;
            int lLoopIndex = 0;
            //RL: Loop through each line of the txt file
            while ((inputLine = oReader.ReadLine()) != null)
            {
                if (bAppendLine && !inputLine.Trim().StartsWith("</") && inputLine.Trim() != "")
                {
                   
                    sPreContent += inputLine + "\n";

                    //RL: We are reading the table headers
                    if (inputLine.Trim().StartsWith("Dy")) //RL: There is an assumption here that the first table column is 'Dy'. If the feed from API changes, this will need to change. We can set this as a config so we don't change the code each time this changes.
                    {
                        lTableColumnsTxtIndex = lLoopIndex;
                        var cols = inputLine.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        for (int col = 0; col < cols.Length; col++)
                            dtWeather.Columns.Add(new DataColumn(cols[col]));

                    } else if (lTableColumnsTxtIndex > -1 && lLoopIndex > lTableColumnsTxtIndex)
                    {
                        var colValues = inputLine.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        bool bAddline = true;
                        Double dblNumber = 0;
                        DataRow dr = dtWeather.NewRow();
                        for (int i = 0; i < colValues.Length; i++)
                        {
                            //RL: We assume that the first column is of type double. If it is not, it must be a dirty data and we will have to exclude it.
                            if (i == 0 && !Double.TryParse(colValues[i], out dblNumber)) 
                            {
                                bAddline = false;
                            }
                            
                            if (i < 3) //RL: We were given information that the first 3 columns are what we needed and that they are of number type
                            {
                                double dblColVal = 0;
                                if (Double.TryParse(colValues[i].Replace("*",""), out dblColVal))
                                {
                                    dr[i] = dblColVal;
                                }
                            } else
                            {
                                dr[i] = colValues[i];
                            }
                            
                        }
                        
                        if (bAddline) dtWeather.Rows.Add(dr);
                    }
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
                lLoopIndex++;
            }

            var sDayWithLeastTempDifference = dtWeather.Select()
                .OrderBy(row => Double.Parse(row.Field<string>("MxT")) - Double.Parse(row.Field<string>("MnT")))
                .FirstOrDefault()["Dy"].ToString();

            Console.Write("Day with the least temperature difference is: day " + sDayWithLeastTempDifference);
            Console.ReadLine();
        }
    }
}
