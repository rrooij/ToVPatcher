﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TownMapDisplay
{
    enum ProgramMode
    {
        None,
        GUI,
        png
    }

    static class Program
    {
        static void PrintUsage()
        {
            Console.WriteLine("FontDisplay");
            Console.WriteLine(" -fontinfofile ffinfo.bin/tov.elf");
            Console.WriteLine(" -fontinfofiletype fontinfo/elf");
            Console.WriteLine(" -textfile text.txt");
            Console.WriteLine(" -mode gui/png");
            Console.WriteLine(" -font FONTTEX10.TXV");
            Console.WriteLine(" -fontblock 0");
            Console.WriteLine(" -outfile out.png");
            Console.WriteLine(" -boxbybox");
            Console.WriteLine(" -dialoguebubble");
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            /*
            FontDisplay
            -fontinfofile tov.elf
            -fontinfofiletype elf/fontinfo
            -textfile text.txt
            -mode gui/png
            -font FONTTEX10
            -fontblock 0
            -outfile text.png
            */

            Util.Path = "ffinfo.bin";
            Util.FontInfoOffset = 0;
            String Textfile = null;
            ProgramMode Mode = ProgramMode.GUI;
            String Font = "FONTTEX10.TXV";
            int Fontblock = 0;
            String Outfile = "out.png";
            bool BoxByBox = false;
            bool DialogueBoxColor = false;

            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLowerInvariant())
                    {
                        case "-fontinfofile":
                            Util.Path = args[++i];
                            break;
                        case "-fontinfofiletype":
                            switch (args[++i].ToLowerInvariant())
                            {
                                case "elf":
                                    Util.FontInfoOffset = 0x00720860;
                                    break;
                                case "fontinfo":
                                    Util.FontInfoOffset = 0;
                                    break;
                            }
                            break;
                        case "-textfile":
                            Textfile = args[++i];
                            break;
                        case "-mode":
                            switch (args[++i].ToLowerInvariant())
                            {
                                case "gui":
                                    Mode = ProgramMode.GUI;
                                    break;
                                case "png":
                                    Mode = ProgramMode.png;
                                    break;
                            }
                            break;
                        case "-font":
                            Font = args[++i];
                            break;
                        case "-fontblock":
                            Fontblock = Int32.Parse(args[++i]);
                            break;
                        case "-outfile":
                            Outfile = args[++i];
                            break;
                        case "-boxbybox":
                            BoxByBox = true;
                            break;
                        case "-dialoguebubble":
                            DialogueBoxColor = true;
                            break;
                    }
                }
            }
            catch ( IndexOutOfRangeException )
            {
                PrintUsage();
                return;
            }



            try
            {
                byte[] File = System.IO.File.ReadAllBytes(Util.Path);

                FontInfo[] f = new FontInfo[6];
                f[0] = new FontInfo(File, Util.FontInfoOffset);
                f[1] = new FontInfo(File, Util.FontInfoOffset + 0x880);
                f[2] = new FontInfo(File, Util.FontInfoOffset + 0x880 * 2);
                f[3] = new FontInfo(File, Util.FontInfoOffset + 0x880 * 3);
                f[4] = new FontInfo(File, Util.FontInfoOffset + 0x880 * 4);
                f[5] = new FontInfo(File, Util.FontInfoOffset + 0x880 * 5);

                String[] TextLines = null;
                if ( Textfile != null ) {
                    TextLines = System.IO.File.ReadAllLines(Textfile);
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Form1 form = new Form1(f, Font, Fontblock, TextLines, BoxByBox, DialogueBoxColor);

                if (Mode == ProgramMode.GUI)
                {
                    Application.Run(form);
                }
                else if (Mode == ProgramMode.png)
                {
                    form.SaveAsPng(Outfile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                PrintUsage();
                return;
            }
        }
    }
}
