using System;
using System.Windows.Forms;

namespace bWake
{
    class Program
    {
        static string _command = null;
        static bool _running = true;

        [STAThread] // for open file dialog
        static void Main(string[] args)
        {
            int hour;
            int minute;
            bool parseSuccess = true;
            bool commandEntered = true;

            if (args.Length < 2)
            {
                // *** manual wake time & command input ***

                string command = null;
                Console.Write("Hour: ");        parseSuccess &= Int32.TryParse(Console.ReadLine(), out hour);
                Console.Write("Minute: ");      parseSuccess &= Int32.TryParse(Console.ReadLine(), out minute);

                do {
                    Console.Write("Command as a path? (y/n): ");
                    if (Console.ReadKey(false).Key == ConsoleKey.Y)
                    {
                        OpenFileDialog dlg = new OpenFileDialog();
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            command = dlg.FileName;
                            Console.WriteLine();
                        }
                        else
                            commandEntered = false;
                    }
                    else
                    {
                        Console.Write("\nCommand: ");     command = Console.ReadLine();
                    }
                } while (!commandEntered);


                if (command != "")
                    _command = command;
            }
            else
            {
                // *** input args parsing ***
                parseSuccess &= Int32.TryParse(args[0], out hour);
                parseSuccess &= Int32.TryParse(args[1], out minute);

                int argpos = 2;
                while (argpos < args.Length)
                {
                    _command += args[argpos] + " ";
                    ++argpos;
                }
            }

            if (!parseSuccess ||
                hour < 0 || hour > 24 ||
                minute < 0 || minute > 60)
            {
                Console.WriteLine( "ERROR: Either hour or minute or both are invalid numbers");
                Console.WriteLine( "  Hour: " + hour + "     Minute: " + minute);
                Console.ReadKey();
                return;
            }



            DateTime now = DateTime.Now;
            DateTime dt = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            if (dt < now)
                dt += new TimeSpan(24, 0, 0);

            WakeUPTimer.WakeUP wup = new WakeUPTimer.WakeUP();
            wup.Woken += On_WakeUp;
            wup.SetWakeUpTime(dt);

            Console.WriteLine( "\n  *** ALARM CLOCK SET ***");
            Console.WriteLine("Wake up at " + dt);
            Console.WriteLine("Command: " + _command);
            Console.WriteLine();

            while (_running)
                System.Threading.Thread.Sleep(1000);

            Console.ReadKey();
        }



        static void On_WakeUp(object sender, EventArgs e)
        {
            Console.WriteLine("*** WAKE UP ***");
            if (_command != null)
            {
                try
                {
                    System.Diagnostics.Process.Start(_command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Your command raised an exception: fuck off");
                    Console.WriteLine(ex);

                    int repetitions = 0;
                    while (!Console.KeyAvailable && repetitions < 10)
                    {
                        Console.WriteLine("\a\a\a");
                        System.Threading.Thread.Sleep(1000);
                        Console.WriteLine("\a");
                        System.Threading.Thread.Sleep(700);
                        Console.WriteLine("\a");
                        System.Threading.Thread.Sleep(700);
                        Console.WriteLine("\a");
                        System.Threading.Thread.Sleep(1000);
                        Console.WriteLine("\a\a\a");
                        System.Threading.Thread.Sleep(2000);

                        repetitions++;
                    }
                }
            }
            _running = false;
        }

    }
}
