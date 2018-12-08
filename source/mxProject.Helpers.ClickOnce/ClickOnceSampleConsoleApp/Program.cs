using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxProject.ClickOnce;

namespace ClickOnceSampleConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {

            try
            {
                AppUpdator updator = new AppUpdator();

                Task.Run(async () =>
                {
                    if (!await updator.UpdateAsync().ConfigureAwait(false))
                    {
                        ClassLibrary1.Class1 class1 = new ClassLibrary1.Class1();
                        Console.WriteLine("Class1.GetName : " + class1.GetName());
                    }
                }
                ).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Done. To quit the application, press any key.");
            Console.ReadLine();

        }


    }
}
