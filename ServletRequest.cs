using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public class ServletRequest
{
    private string request;
    private ArrayList reqBytes;
    private string[] lines;
    private int firstLine = -1;
    private int lastLine = -1;

    public ServletRequest(string req, ArrayList bytes){
        this.request = req;
        this.reqBytes = bytes;
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
                string[] names = lines[i].Split(new string[] { "filename=\"" }, StringSplitOptions.None);
                fileName = names[1].Substring(0, names[1].Length - 2);
                this.firstLine = i + 3;
                break;
            }
        }

        return fileName.Split('.')[0];
    }
    public string getCaption(){
        String cap = "";
        
        for(int i = 0; i < lines.Length; i++){
            if(lines[i].Contains("name=\"caption\"")){
                cap = lines[i + 2];
                this.lastLine = i - 2;
                break;
            }
        }

        if (string.IsNullOrEmpty(cap.Trim())) {
            cap = "No caption";
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

        if (string.IsNullOrEmpty(date.Trim())) {
            DateTime now = DateTime.Now;
            date = now.Year + "-" + now.Month + "-" + now.Day;
        }

        return date;
    }

    public ArrayList getBytes(){
        ArrayList fileBytes = new ArrayList();
        int begPos = 0, endPos;
        if(this.firstLine > 0 && this.lastLine > 0){
            for(int i = 0; i < firstLine; i++){
                begPos += lines[i].Length + 1;
            }

            endPos = begPos;
            for(int i = firstLine; i <= lastLine; i++){
                endPos += lines[i].Length + 1;
            }

            for(int i = begPos; i <= endPos; i++){
                fileBytes.Add(this.reqBytes[i]);
            }
        }

        return fileBytes;
    }
}
