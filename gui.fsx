open System
open System.IO
open System.Text.RegularExpressions

// Function to read text from a file
let loadTextFromFile (filePath: string) =
    try
        File.ReadAllText(filePath)
    with
    | ex -> sprintf "Error reading file: %s" ex.Message

let getUserInput () =
    printfn "Choose input method: 1 - Direct Text, 2 - upload File using path"
    match Console.ReadLine() with
    | "1" -> 
        printfn "Enter your text:"
        Console.ReadLine()
    | "2" -> 
        printfn "Enter the file path:"
        let filePath = Console.ReadLine()
        if File.Exists filePath then
            loadTextFromFile(filePath)
        else
            printfn "File not found. Exiting."
            ""
    | _ -> 
        printfn "Invalid choice. Exiting."
        ""
let cleanText (text: string) =
   // Keep "F#" as a special case and remove other punctuation
   let cleaned = Regex.Replace(text, @"[^\w\s#]", "")
   cleaned

let countWords (text: string) =
    text.Split([|' '; '\n'; '\t'; '\r'|], StringSplitOptions.RemoveEmptyEntries).Length
