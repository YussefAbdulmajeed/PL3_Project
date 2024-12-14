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
    
    // Set the form to be maximized on startup
    form.WindowState <- FormWindowState.Maximized

    // Create buttons and other controls
    let uploadButton = new Button(Text = "Upload File", Location = Point(20, 20), Size = Size(160, 50), BackColor = Color.CadetBlue, ForeColor = Color.Black)
    let analyzeButton = new Button(Text = "Analyze Text", Location = Point(200, 20), Size = Size(160, 50), BackColor = Color.CadetBlue, ForeColor = Color.Black)
    let clearButton = new Button(Text = "Clear", Location = Point(380, 20), Size = Size(160, 50), BackColor = Color.Red, ForeColor = Color.White)
    let wordCountButton = new Button(Text = "Word Count", Location = Point(20, 90), Size = Size(160, 50), BackColor = Color.LightSkyBlue, ForeColor = Color.Black)
    let sentenceCountButton = new Button(Text = "Sentence Count", Location = Point(200, 90), Size = Size(160, 50), BackColor = Color.LightSkyBlue, ForeColor = Color.Black)
    let paragraphCountButton = new Button(Text = "Paragraph Count", Location = Point(380, 90), Size = Size(160, 50), BackColor = Color.LightSkyBlue, ForeColor = Color.Black)
    let wordFreqButton = new Button(Text = "Word Frequency", Location = Point(560, 90), Size = Size(160, 50), BackColor = Color.LightSkyBlue, ForeColor = Color.Black)
    let avgSentenceLengthButton = new Button(Text = "Avg Sentence Length", Location = Point(740, 90), Size = Size(160, 50), BackColor = Color.LightSkyBlue, ForeColor = Color.Black)
    
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

// Button for file upload with updated functionality
    uploadButton.Click.Add(fun _ -> 
        let openFileDialog = new OpenFileDialog()
        openFileDialog.Filter <- "Text files (.txt)|*.txt|Word documents (.docx)|*.docx|All files (*.*)|*.*" // Multiple types
        openFileDialog.Title <- "Select a File"
    
        if openFileDialog.ShowDialog() = DialogResult.OK then
            let filePath = openFileDialog.FileName
            // Load the file's content based on its extension
            let content = 
                match Path.GetExtension(filePath).ToLower() with
                | ".txt" -> loadTextFromFile filePath
                | ".docx" -> 
                    // Add support for reading .docx files (you will need to use a library like Open XML SDK or NPOI)
                    "Word files not yet supported in this example"
                | _ -> "File type not supported."
        
            // Display content or filename in the textbox
            textBox.Text <- content // If it’s a txt file, it will display content
            // Optionally, display just the file name:
            // textBox.Text <- Path.GetFileName(filePath) 
    )
    analyzeButton.Click.Add(fun _ ->
        let text = textBox.Text
        if String.IsNullOrWhiteSpace(text) then
            MessageBox.Show("No text provided.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        else
            // Perform analysis
            let wordCount = countWords text
            let sentenceCount = countSentences text
            let paragraphCount = countParagraphs text
            let avgSentenceLen = averageSentenceLength text
            let freq = 
                wordFrequency text
                |> Seq.take 5 // Show top 5 words
                |> Seq.map (fun (word, count) -> sprintf "  • %s: %d" word count) // Bullet points for words
                |> String.concat "\n"

            // Organize output with clear sections
            let outputText =
                sprintf
                    """Text Analysis Report
                    Word Count            : %d
                    Sentence Count        : %d
                    Paragraph Count       : %d
                    Average Sentence Length: %.2f words/sentence
                    Most Frequent Words:
                    %s
                    """
                    wordCount
                    sentenceCount
                    paragraphCount
                    avgSentenceLen
                    freq
            // Set the output text to the output box
            outputBox.Text <- outputText
    )