using FrenzyEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FrenzyEditor.GameProject
{
    [DataContract] //Allows to serialize priate fields
    public class ProjectTemplate
    {
        [DataMember]
        public string? ProjectType { get; set; }
        [DataMember]
        public string? ProjectFile { get; set; }
        [DataMember]
        public List<string>? Folders { get; set; }
        public byte[]? Icon { get; set; }
        public byte[]? Screenshot { get; set; }
        public string? IconFilePath { get; set; }
        public string? ScreenshotFilePath { get; set; }
        public string? ProjectFilePath { get; set; }
    }
    public class NewProject : ViewModelBase
    {
        //TODO: get the path from the intilation location
        private readonly string _templatePath = @"..\..\FrenzyEditor\ProjectTemplates";
        private string _projectName = "NewProject";
        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (_projectName != value)
                {
                    _projectName = value;
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }

        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\FrenzyProject\";
        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _priojectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate>? ProjectTemplates { get; }
        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_priojectTemplates);
            try
            {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                foreach (var templateFile in templateFiles) 
                {
                    var template = Serializer.FromFile<ProjectTemplate>(templateFile);
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templateFile), "Icon.jpg"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templateFile), "Screenshot.jpg"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templateFile), template.ProjectFile));

                    _priojectTemplates.Add(template);
                }
            }
            catch (Exception ex)
            { 
                Debug.WriteLine(ex.Message);
                //TODO: Log errors in a better way
            }
        }
    }
}
