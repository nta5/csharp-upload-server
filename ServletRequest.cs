using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public class ServletRequest
{
    private string reqeust;
    private string[] lines;

    public ServletRequest(string req){
        this.reqeust = req;
        lines = req.Split('\n');

        // for(int i = 0; i < lines.Length; i++){
        //     Console.WriteLine("Line Number: " + i + lines[i]);
        // }
    }
    
    public string getFileName(){
        String fileName = "";

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("filename"))
            {
                string[] names = lines[i].Split(new string[] { "filename=\"" }, StringSplitOptions.RemoveEmptyEntries);
                fileName = names[1].Substring(0, names[1].Length - 2);
                break;
            }
        }

        return fileName;
    }
    public string getCaption(){
        String cap = "";
        
        for(int i = 0; i < lines.Length; i++){
            if(lines[i].Contains("name=\"caption\"")){
                cap = lines[i + 2];
                break;
            }
        }

        return cap;
    }

    public string getDate()
    {
        String date = "";

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("name=\"date\""))
            {
                date = lines[i + 2];
                break;
            }
        }

        return date;
    }
}
