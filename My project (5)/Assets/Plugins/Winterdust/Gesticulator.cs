using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Python.Runtime;




namespace Gesticulator
{

    public class PyGesticulatorTestor_Avatar
    {

        static PyGesticulatorTestor_Avatar()
        {
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", @"/Users/yunsu-in/Desktop/metaverse_termproject/metaverse_termproject/My project (5)/Assets/CS_DLL/python37.dll", EnvironmentVariableTarget.Process); 
            var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"/Users/yunsu-in/opt/anaconda3/envs/meta2");

            Debug.Log("PythonHome ="+PYTHON_HOME);
            PythonEngine.PythonHome = PYTHON_HOME;
            Debug.Log("PythonEngine.PythonPath ="+PythonEngine.PythonPath);
            PythonEngine.PythonPath = string.Join
            (
                Path.PathSeparator.ToString(),
                new string[]
                {
            
                        PythonEngine.PythonPath,
                        Path.Combine(PYTHON_HOME, @"Lib\site-packages"),
                        @"/Users/yunsu-in/Desktop/metaverse_termproject/metaverse_termproject/My project (5)/Assets/gesticulator/demo",
                        @"/Users/yunsu-in/Desktop/metaverse_termproject/metaverse_termproject/My project (5)/Assets/whisper-main",
                }
            );
        
            Debug.Log(PythonEngine.PythonPath);
            PythonEngine.Initialize();
        }
        public static void main()
        {
            using (Py.GIL())
            {
                PythonEngine.RunSimpleString(@"
                                                import sys;
                                                print('Hello world');
                                                print(sys.version);
                ");

                dynamic pysys = Py.Import("sys");   // It uses  PythonEngine.PythonPath 
            
                dynamic pySysPath = pysys.path;
                Debug.Log("pySysPath =" + pySysPath);

                string[] sysPathArray = ( string[]) pySysPath;
                Debug.Log("sysPathArray = " + sysPathArray);
                List<string> sysPath = ((string[])pySysPath).ToList<string>();
                Console.WriteLine("\nsys.path:\n");
                Array.ForEach(sysPathArray, element =>  Console.Write("{0}\t", element));
                dynamic os = Py.Import("os");

                dynamic pycwd = os.getcwd();
                string cwd = (string)pycwd;

                Console.WriteLine("\n\n initial os.cwd={0}", cwd);


                cwd = @"/Users/yunsu-in/Desktop/metaverse_termproject/metaverse_termproject/My project (5)/Assets/gesticulator/demo";

                Console.WriteLine("\n\n new os.cwd={0}", cwd, "\n\n");
                dynamic gesture = Py.Import("demo");

            }
            PythonEngine.Shutdown();
        }


    
    } 
} 


