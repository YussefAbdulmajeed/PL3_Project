open System
open System.IO
open System.Text.RegularExpressions
open System.Windows.Forms
open System.Drawing

// Function to read text from a file
let loadTextFromFile (filePath: string) =
    try
        File.ReadAllText(filePath)
    with
    | ex -> sprintf "Error reading file: %s" ex.Message

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

// Function to display results
let displayResults (text: string) =
    let results = 
        [
            sprintf "Word count: %d" (countWords text)
            sprintf "Sentence count: %d" (countSentences text)
            sprintf "Paragraph count: %d" (countParagraphs text)
        ]
    let wordFreq = wordFrequency text 
                   |> Seq.take 10 
                   |> Seq.map (fun (word, count) -> sprintf "%s: %d" word count) 
                   |> String.concat "\n"
    let avgSentenceLength = sprintf "Average Sentence Length: %.2f" (averageSentenceLength text)
    
    // Concatenate all results into one string
    String.concat "\n" results + "\n\nMost Frequent Words:\n" + wordFreq + "\n" + avgSentenceLength

// GUI function
let createForm () =
    let form = new Form(Text = "Text Analysis Tool", ClientSize = Size(600, 600),BackColor=Color.Black)

    
    let textBox = new TextBox(
        Text = "Input", 
        Location = Point(20, 160), 
        Size = Size(1100, 200), 
        Multiline = true, 
        ScrollBars = ScrollBars.Vertical, 
        Font = new Font("Arial", 14f)
    )
    let outputBox = new TextBox(
        Text = "Output", 
        Location = Point(20, 380), 
        Size = Size(540, 200), 
        Multiline = true, 
        ScrollBars = ScrollBars.Vertical, 
        ReadOnly = true, 
        Font = new Font("Arial", 14f)
    )


