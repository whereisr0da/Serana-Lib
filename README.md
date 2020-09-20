# Serana 0.3

Serana is a .NET library that can parse windows executables

This library interpret all elements of the executable and represent them in a Object Programming way

So you can get any informations of a PE structure and modify each one of them (in the future) 

These objects could be exported (raw file buffers) separately after modifying them or export the entire executable

## Why ?

I know, I know ... there is a lot of library like this.

I like use pefile in python, but in .NET there is not that much about PE files, there is libs for .NET such as Mono, dnlib.

I made this to improve my PE knowledge.


## How ?

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

// close handles
pe.Dispose();
```

You can export parts of the executable
```c#
PE pe = new PE("app.exe");

// get the exported optional header raw buffer
byte[] optionalHeader = pe.header.optionalHeader.export().ToArray<byte>();

// doing something with it ...

// close handles
pe.Dispose();
```

You can modify the executable
```c#
PE pe = new PE("app.exe");

// update the executable subsystem
pe.header.optionalHeader.peSubSystem.setValue(SubSystem.NATIVE_WINDOWS);

// fix the stack size
pe.header.optionalHeader.SizeOfStackReserve.setValue(0x1000);

// get some data
byte[] virtualizedCode = ...

// add a new section
pe.sections.addSection(".vlizer", virtualisedCode, SectionTypes.DATA_SECTION);

// write the output executable
File.WriteAllBytes("app_obfuscated.exe", pe.export().ToArray<byte>());

// close handles
pe.Dispose();
```

## Changelog

Version 0.3

* Resource parser implemented (modifications not available)
* Section adding fixed for x86 (still buggy for x64)
* Some functions added
* Some bug fixed

Version 0.2

* Export after parse x86 / x64 work
* Implement modification 😃
* Implement PE file creation from memory
* Implement section adding (buggy)
* Improved x64 support
* Code cleaning
* More comments / documentation
* More improvements ...

Version 0.1

* Initial release

## Issues

This library is IN BETA, so bugs can be found.

## TODO

- Handle imports, exports
- Fix export problems for x64
- Fix section adding for x64
- Some other fix
- DOCUMENTATION !!!

## License
[Creative Commons Attribution-NonCommercial-NoDerivatives](http://creativecommons.org/licenses/by-nc-nd/4.0/)
