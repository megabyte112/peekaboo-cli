using System.Text;

namespace peekaboo_cli;

internal static class Program
{
    private const string Version = "v0.0.1a";

    private static void Main(string[] args)
    {
        switch (args.Length)
        {
            case 1:
                Decode(args[0]);
                break;
            case 2:
                Encode(args[0], args[1]);
                break;
            default:
                throw new ArgumentException();
        }
    }

    private static void Encode(string image, string file)
    {
        // output filename
        const string filename = "output.png";
        
        // remove output if file already exists
        if (File.Exists(filename)) File.Delete(filename);
        
        // create stream
        var stream = File.Open(filename, FileMode.CreateNew);
        var bw = new BinaryWriter(stream);

        // get all data to write
        var imageData = File.ReadAllBytes(image);
        var middleText = "/peekaboo/" + Version + "/" + image + "/" + file + "/peekaboo/";
        var fileData = File.ReadAllBytes(file);
        
        // write!
        bw.Write(imageData);
        bw.Write(Encoding.Latin1.GetBytes(middleText));
        bw.Write(fileData);
        
        stream.Close();
        bw.Close();
    }

    private static void Decode(string file)
    {
        // read the whole file and split at the join point
        var fileData = File.ReadAllBytes(file);
        
        // get end point of image - the start of peekaboo metadata
        var stringData = Encoding.Latin1.GetString(fileData);
        var splitData = stringData.Split("/peekaboo/");

        var imageData = Encoding.Latin1.GetBytes(splitData[0]);
        var metaData = splitData[1];
        var dataData = Encoding.Latin1.GetBytes(splitData[2]);

        var splitMetaData = metaData.Split("/");

        var peekabooVersion = splitMetaData[0];
        var imageName = splitMetaData[1];
        var dataName = splitMetaData[2];

        // remove files if they already exist
        if (File.Exists(imageName)) File.Delete(imageName);
        if (File.Exists(dataName)) File.Delete(dataName);
        
        // write image data
        var imageStream = File.Open(imageName, FileMode.CreateNew);
        var bwImage = new BinaryWriter(imageStream);
        bwImage.Write(imageData);
        imageStream.Close();
        bwImage.Close();
        
        // write file data
        var dataStream = File.Open(dataName, FileMode.CreateNew);
        var bwData = new BinaryWriter(dataStream);
        bwData.Write(dataData);
        dataStream.Close();
        bwData.Close();
    }
}
