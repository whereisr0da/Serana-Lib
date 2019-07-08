## Serana

Serana is a .NET library that can parse windows executables

This library interpret all elements of the executable and represent them in a Object Programming way

So you can get any informations of a PE structure and modify each one of them (in the future) 

These objects could be exported (raw file buffers) separately after modifying them or export the entire executable

## Why

I know, I know ... there is a lot of library like this.

I like use pefile in python, but in .NET there is not that much about PE files, there is libs for .NET such as Mono, dnlib.

I made this to improve my PE knowledge.

## Currently

Now this library support only the reading / parsing of executables. So you can use this to take informations about all structures of the PE. The export function work but its useless because nothing can be modify.

## How

You can read informations about the executable
```c#
// init the PE object
PE pe = new PE("app.exe");

// get info about architecture
Console.WriteLine("is32bit : " + pe.header.is32Bit);

// get info about image base

// get the offset of the raw image base in file
int imageBaseOffset = pe.header.optionalHeader.ImageBase.getOffset();

// get the value of image base
int imageBase = pe.header.optionalHeader.ImageBase.getValue();

Console.WriteLine("ImageBase at 0x" + imageBaseOffset.ToString("X") + " : 0x" 
    + imageBase.ToString("X"));

// print info about sections
Console.WriteLine("Sections : ");

// loop through all sections
foreach (SectionEntry section in pe.sections.sectionEntries)
{
    // get section name
    string name = section.name.ToString();
    // get section data offset
    int dataOffset = section.pointerToRawData.getValue();
    // get section data size
    int size = section.sizeOfRawData.getValue();
    // get raw data of section
    byte[] sectionBuffer = section.getSectionBuffer();

    Console.WriteLine(name + " at 0x" + dataOffset.ToString("X") 
      + " (size : 0x" + size.ToString("X") + ")");
}

// close the stream
pe.Dispose();
```

You can export parts of the executable
```c#
PE pe = new PE("app.exe");

// get the exported optional header raw buffer
byte[] optionalHeader = pe.header.optionalHeader.export().ToArray<byte>();

// modify something... (in the future)

// write the output executable
File.WriteAllBytes("appModded.exe", pe.export().ToArray<byte>());

pe.Dispose();
```

## Issues

This library is IN BETA, so bugs can be found.

## TODO

- Some fix
- Support modification (not yet)

## License
[Creative Commons Attribution-NonCommercial-NoDerivatives](http://creativecommons.org/licenses/by-nc-nd/4.0/)
