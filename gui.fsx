open System
open System.IO
open System.Text.RegularExpressions

// Function to read text from a file
let loadTextFromFile (filePath: string) =
    try
        File.ReadAllText(filePath)
    with
    | ex -> sprintf "Error reading file: %s" ex.Message

// this is user interface function to determine the method
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

// this functoin to clean text from special character
let cleanText (text: string) =
   // Keep "F#" as a special case and remove other punctuation
   let cleaned = Regex.Replace(text, @"[^\w\s#]", "")
   cleaned

// this function count frequency of words
let countWords (text: string) =
    text.Split([|' '; '\n'; '\t'; '\r'|], StringSplitOptions.RemoveEmptyEntries).Length

let countSentences (text: string) =
    // text.Split([|'.'; '!'; '?'|], StringSplitOptions.RemoveEmptyEntries).Length
    let sentences = Regex.Split(text, @"(?<=[.!?])\s+") 
    let filteredSentences = sentences |> Array.filter (fun s -> s.Trim().Length > 0)
    filteredSentences.Length

// function to handle paragraph counting in text processing
let countParagraphs (text: string) =
    text.Split([|'\n'; '\r'|], StringSplitOptions.RemoveEmptyEntries).Length

// Function to calculate word frequency
let wordFrequency (text: string) =
    let words = Regex.Matches(text.ToLower(), @"\w+")
    words
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Value)
    |> Seq.countBy id
    |> Seq.sortByDescending snd

// // Function to measure readability (average sentence length)
let averageSentenceLength (text: string) =
    let cleandText = cleanText text
    let wordCount = countWords cleandText
    let sentenceCount = countSentences text
    if sentenceCount > 0 then float wordCount / float sentenceCount
    else 0.0

// fuction to display results
let displayResults (text: string) =
   printfn "Text Analysis Results:"
   printfn "-----------------------"
   printfn "Word count: %d" (countWords text)
   printfn "Sentence count: %d" (countSentences text)
   printfn "Paragraph count: %d" (countParagraphs text)
   printfn "\nMost Frequent Words:"
   wordFrequency text
   |> Seq.take 10
   |> Seq.iter (fun (word, count) -> printfn "%s: %d" word count)
   printfn "\nAverage Sentence Length: %.2f" (averageSentenceLength text)

// function to handle user input to display results
let main argv =
    let text = getUserInput()
    if not (String.IsNullOrWhiteSpace text) then
        displayResults text
    else
        printfn "No text provided."
    0

// Call main explicitly for FSI
main [||] |> ignore

