using System;

namespace VSProjectCleaner
{
    using System.IO;

    using Microsoft.Build.Evaluation;

    internal class CsProjectFileProcessor
    {
        private readonly string projectFilePath;

        private readonly Project project;

        private bool bChanged;
        public CsProjectFileProcessor(string csProjectFilePath)
        {
            if ( !File.Exists(csProjectFilePath))
                throw new ArgumentException(".csproj file does not exist");

            projectFilePath = csProjectFilePath;
            project = new Project( csProjectFilePath );
        }

        public bool Save()
        {
            if (bChanged)
            {
                project.Save();
            }

            return bChanged;
        }

        public void ProcessProject(Settings settings)
        {
            Console.WriteLine("Project File : {0}",  Path.GetFileName(this.projectFilePath ));
            Console.WriteLine("Path : {0}",  Path.GetDirectoryName(this.projectFilePath));
            Console.WriteLine("--------------------------------------------------------------");

            this.StepSignAssembly(settings);
            this.StepEnableCodeAnalysis(settings);

            Console.WriteLine("--------------------------------------------------------------");
        }


        private void StepSignAssembly( Settings settings)
        {           
            string keyPath = PathHelper.GetRelativePath( Path.GetFullPath(projectFilePath), settings.SnkFilePath);
            this.UpdateProjectProperty("AssemblyOriginatorKeyFile", keyPath);
            this.UpdateProjectProperty("SignAssembly", "true");
        }

        private void StepEnableCodeAnalysis(Settings settings)
        {
            this.UpdateProjectProperty("RunCodeAnalysis","true");
            this.UpdateProjectProperty("CodeAnalysisIgnoreGeneratedCode","true");
            this.UpdateProjectProperty("CodeAnalysisRuleSet", "MinimumRecommendedRules.ruleset");
            this.UpdateProjectProperty("TreatWarningsAsErrors","true");
        }

        private void UpdateProjectProperty(string key, string value)
        {
            var keyProperty = project.GetProperty(key);

            if (keyProperty != null)
            {
                if (keyProperty.UnevaluatedValue != value)
                {
                    Console.WriteLine("    Changing Property : {0} = {1}", key, value);

                    keyProperty.UnevaluatedValue = value;
                    this.bChanged = true;
                }
            }
            else
            {
                Console.WriteLine("    Adding Property : {0} = {1}", key, value);

                project.SetProperty(key, value);
                bChanged = true;
            }            
            
        }



    }
}
