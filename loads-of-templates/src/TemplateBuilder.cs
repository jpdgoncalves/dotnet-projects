
using System.Text;

namespace TemplateBuilder.Core {

    public class TemplateBuilder : IDirectoryBuilder, IBuilder
    {
        private readonly string _outPath;
        private readonly bool _createDir;
        private readonly List<IBuilder> _subBuilders = new();

        public TemplateBuilder(string outDir, bool createDir = false) {
            _outPath = Path.GetFullPath(outDir);
        }

        public IDirectoryBuilder AddDirectory(string path)
        {
            var subDir =  new DirectoryBuilder(
                Path.GetFullPath(Path.Join(_outPath, path))
            );
            _subBuilders.Add(subDir);
            return subDir;
        }

        public IFileBuilder AddFile(string path)
        {
            var file = new FileBuilder(
                Path.GetFullPath(Path.Join(_outPath, path))
            );
            _subBuilders.Add(file);
            return file;
        }

        public void Build()
        {
            var _dirExists = Directory.Exists(_outPath);

            if (_createDir && _dirExists) {
                Console.Error.WriteLine($"Output directory '{_outPath}' already exists and createDir Flag is set to true.");
                return;
            }

            if (!_createDir && !_dirExists) {
                Console.Error.WriteLine($"Output directory '{_outPath}' doesn't exist and createDir Flag is set to true.");
                return;
            }

            try {
                _subBuilders.ForEach(b => b.Build());
            } catch (Exception exception) {
                Console.Error.WriteLine(exception.Message);
            }
        }
    }

    public interface IBuilder {
        public void Build();
    }

    public interface IDirectoryBuilder {
        public IDirectoryBuilder AddDirectory(string path);
        public IFileBuilder AddFile(string path);
    }

    public interface IFileBuilder {
        public void AddContent(string content);
    }

    internal class DirectoryBuilder : IDirectoryBuilder, IBuilder
    {
        private readonly string _basePath;
        private readonly List<IBuilder> _subBuilders = new();

        public DirectoryBuilder(string path) {
            _basePath = path;
        }

        public IDirectoryBuilder AddDirectory(string path)
        {
            var subDir =  new DirectoryBuilder(
                Path.GetFullPath(Path.Join(_basePath, path))
            );
            _subBuilders.Add(subDir);
            return subDir;
        }

        public IFileBuilder AddFile(string path)
        {
            var file = new FileBuilder(
                Path.GetFullPath(Path.Join(_basePath, path))
            );
            _subBuilders.Add(file);
            return file;
        }

        public void Build()
        {
            Directory.CreateDirectory(_basePath);
            Console.WriteLine($"Created directory {_basePath}");

            _subBuilders.ForEach(b => b.Build());
        }
    }

    internal class FileBuilder : IFileBuilder, IBuilder
    {
        private static readonly FileStreamOptions _options = new FileStreamOptions() {
            Mode = FileMode.CreateNew
        };

        private readonly string _path;
        private readonly StringBuilder _content = new();

        public FileBuilder(string path) {
            _path = path;
        }
        public void AddContent(string content)
        {
            _content.Append(content);
        }

        public void Build()
        {
            using (var file = new StreamWriter(_path, _options)) {
                file.Write(_content.ToString());
            }

            Console.WriteLine($"Created file at '{_path}'");
        }
    }
}