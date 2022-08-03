//This program is a simple compiler for a custom programming language called Integer-Oriented Language (IOL).
//A simplified custom language that only involves integer-type values as the numeric values and numerical operations and expressions.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace _129_Final_Project__TONACAO_
{

    public partial class Form1 : Form
    {
        string newline = Environment.NewLine;
       
        public Form1()
        {
            InitializeComponent();
           
        }
        bool loaded = false;
        string filePath;
        //event button for Open File that opens file directory displaying .iol files
        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofile = new OpenFileDialog();
            ofile.Filter = "Input file (*.iol)|*.iol;";// display only .iol files

            if (DialogResult.OK == ofile.ShowDialog())
            {
                //read .iol file line per line
                var lines = File.ReadAllLines(ofile.FileName);
                string input = "";

                for (int i = 0; i < lines.Length; i++)
                {
                    input = input + lines[i] + newline;
                }

                //displays .iol content to editable textbox
                richTextBox1.Text = input;
                string newinput = richTextBox1.Text;
                string result = newinput.Remove(newinput.Length - 1);
                richTextBox1.Text = result;
                loaded = true;
                //displays file name of loaded .iol file in the window title
                filePath =ofile.FileName;
                Form1.ActiveForm.Text = Path.GetFileName(filePath); 

            }

            //disable tokenized and execute button for new loaded file until compiled
            showTokenizedToolStripMenuItem.Enabled = false;
            executeToolStripMenuItem.Enabled = false;
        }

        //Save contents of code editor textbox to a new file
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofile = new OpenFileDialog();
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = ofile.Filter = "IOL FILE | *.iol;";
            if (save.ShowDialog() == DialogResult.OK) 
            {
                using (StreamWriter textOutput = new StreamWriter(save.FileName))
                {
                    textOutput.Write(richTextBox1.Text);
                    textOutput.Close();
                }
            }
            //displays file name of loaded .iol file in the window title
            filePath = save.FileName;
            Form1.ActiveForm.Text = Path.GetFileName(filePath);
        }

       //Event button to save currently opened .iol file
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofile = new OpenFileDialog();

                if (File.Exists(filePath))
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        string newText = string.Empty;
                        for (int i = 0; i < richTextBox1.Lines.Length; i++)
                        {
                            newText += richTextBox1.Lines[i];
                            if (i != richTextBox1.Lines.Length - 1)
                                newText += "\n";

                        }
                        sw.Write(newText);
                        sw.Close();
                    }

                //display current file name in tab title
                //filePath = ofile.FileName;
                Form1.ActiveForm.Text = Path.GetFileName(filePath);
            }

                // Same as Save As Implementation
                else
                {
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = ofile.Filter = "IOL FILE | *.iol;";
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter textOutput = new StreamWriter(save.FileName))
                        {
                            textOutput.Write(richTextBox1.Text);
                            textOutput.Close();
                        }
                    }

                //display current file name in tab title
                //filePath = ofile.FileName;
                Form1.ActiveForm.Text = Path.GetFileName(filePath);
            }  
        }

        string[] rawInput; //string array for the input in the code editor text box with white spaces
        
        List<string> lexemes = new List<string>(); //list of lexemes from input
        List<Word> lexemesParse = new List<Word>(); //list of lexemes from input for parsing
        List<string> varName = new List<string>(); //list of variable names for display in datatable
        List<string> varType = new List<string>();  //list of corresponding variable types for display in datatable
        List<string> KeyWords = new List<string> { "INTO", "IS", "BEG", "PRINT", "ADD", "SUB", //list of keywords 
                                                    "MULT", "DIV", "MOD", "INT", "STR", "NEWLN" };

        //Event button for Compile 
        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var input = richTextBox1.Text;
            input = input.Replace(" ", "");

            if (input=="") {// do not compile if empty
                richTextBox2.Text="\n Code editor is empty.";
            }
            else
            {
                // initialize parsing variable for lexemes
                lexemesParse = new List<Word>();

                read(); //reads code input in code editor
                ProcessCode();

                // display table of variables
                DataTable varElementsTable = new DataTable();
                dataGridView1.DataSource = null;

                varElementsTable.Clear();
                varElementsTable.Columns.Clear();
                varElementsTable.Rows.Clear();

                varElementsTable.Columns.Add("Name");
                varElementsTable.Columns.Add("Type");

                for (int i = 0; i < varName.Count; i++)
                {
                    varElementsTable.Rows.Add(varName[i], varType[i]);
                }

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.DataSource = varElementsTable;
            }

           

        }
        private void read(){
  
            rawInput = richTextBox1.Lines; //saves code in an aray line by line

            //create list for variables
            varName = new List<string>();
            varType = new List<string>();
          


            for (int i = 0; i < rawInput.Length; i++){        
                string input = rawInput[i].Trim(); //remove white spaces per line 
                string[] Line = input.Split(' ');
                

                //save lexemes in list
                foreach (string item in Line){
                    //stores each word in list
                    lexemes.Add(item);     
                    lexemesParse.Add(new Word(item, i + 1));
                }
            }
            return;
        }


        private void ProcessCode() {
            string FILENAME = Path.GetFileName(filePath);

            Regex isIdentity = new Regex("^[A-Za-z][A-Za-z0-9allInput]*$"); //definition for a regular expression that is an identity or a letter/digit
            Regex isDigit = new Regex("^[0-9]*$"); //definition for a regular expression that is an digit
            //check if first and last lexeme in list is IOL/LOI
            if ((lexemesParse[0].lexeme == "IOL") && (lexemesParse[lexemesParse.Count - 1].lexeme == "LOI")){

                for (int i = 1; i < lexemesParse.Count - 1; i++){

                    //Tokenize lexeme
                        if (KeyWords.Contains(lexemesParse[i].lexeme) == true){
                            lexemesParse[i].token = lexemesParse[i].lexeme;
                        } else if (isDigit.IsMatch(lexemesParse[i].lexeme) == true) {
                            lexemesParse[i].token = "INT_LIT";
                        } else if (isIdentity.IsMatch(lexemesParse[i].lexeme) == true){
                            lexemesParse[i].token = "VAR_NAME";
                        } else {
                            lexemesParse[i].token = "ERR_LEX";
                        }
                    //end of tokenization

                    if (lexemesParse[i].lexeme == "INT" || lexemesParse[i].lexeme == "STR"){
                        varType.Add(lexemesParse[i].lexeme);
                        if (isIdentity.IsMatch(lexemesParse[i + 1].lexeme)){
                            varName.Add(lexemesParse[i + 1].lexeme);
                                                   }
                        else{
                            varName.Add("Error: not specified");
                        }
                    }
                }

                //enable button for showing tokenized code
                showTokenizedToolStripMenuItem.Enabled = true;

                Parse();
            }
            else
            {
                richTextBox2.Text = FILENAME + " failed to compile...\n";
                richTextBox2.Text = richTextBox2.Text + "Error: No starting and/or ending markers (IOL/LOI) ...";
                executeToolStripMenuItem.Enabled = false;
                showTokenizedToolStripMenuItem.Enabled = false;
            }
            return;
        }
        
    
        List<string> Errors; //list for errors
        int pIndex;//current parsing index
        Word pCurrent; //current lexeme

        private void Parse()
        {

            string FILENAME = Path.GetFileName(filePath);
            //initialize variables
            Errors = new List<string>();
            pIndex = 1; 
            pCurrent = lexemesParse[pIndex]; 

            //process until not LOI
            while (pCurrent.lexeme != "LOI") {
                checkStatement();

                //moves to next index and lexeme
                if (pCurrent.lexeme != "LOI") {
                    pIndex++;
                    pCurrent = lexemesParse[pIndex];
                }
            }

            // if no errors are found
            if (Errors.Count == 0) {
                richTextBox2.Text = FILENAME + " successfully compiled!";
                executeToolStripMenuItem.Enabled = true;
            }
            //if errors are found
            else{
                richTextBox2.Text = FILENAME + " unsuccessfull compilation...";
                executeToolStripMenuItem.Enabled = false;
                showTokenizedToolStripMenuItem.Enabled = false;
                // display list of errors
                foreach (string e in Errors)
                {
                    richTextBox2.Text = richTextBox2.Text + "\n" + e;
                }
            }
        }

        //helper function for parsing
        //identifies whether current token is an arithmetic, operation or a definition
        List<string> artithmeticKeyword = new List<string> { "ADD", "SUB", "MULT", "DIV", "MOD" };
        List<string> operationKeyword = new List<string> { "INTO", "BEG", "PRINT" };
        private void checkStatement() {
            // if arithmetic operation
            if (artithmeticKeyword.Contains(pCurrent.token) || pCurrent.token == "INT_LIT" || pCurrent.token == "VAR_NAME") {
                Arith();
            }

            // for operations
            else if (operationKeyword.Contains(pCurrent.token)) {
                if (pCurrent.token == "INTO") {
                    //moves to next index and lexeme
                    if (pCurrent.lexeme != "LOI"){
                        pIndex++;
                        pCurrent = lexemesParse[pIndex];
                    }

                    if (pCurrent.token == "VAR_NAME") {
                        //moves to next index and lexeme
                        if (pCurrent.lexeme != "LOI") {
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                        }

                        if (pCurrent.token == "IS")  {
                            //moves to next index and lexeme
                            if (pCurrent.lexeme != "LOI") {
                                pIndex++;
                                pCurrent = lexemesParse[pIndex];
                            }
                            Arith();
                            return;
                        }
                    }
                    Errors.Add("Error with Operation in Line: " + pCurrent.lineNo);
                } else if (pCurrent.token == "BEG"){
                    //moves to next index and lexeme
                    if (pCurrent.lexeme != "LOI") {
                        pIndex++;
                        pCurrent = lexemesParse[pIndex];
                    }

                    if (pCurrent.token == "VAR_NAME")
                        return;
                    Errors.Add("Error with Operation in Line: " + pCurrent.lineNo);
                
                }else if (pCurrent.token == "PRINT"){
                    //moves to next index and lexeme
                    if (pCurrent.lexeme != "LOI") {
                        pIndex++;
                        pCurrent = lexemesParse[pIndex];
                    }
                    Arith();
                
                }else {
                    Errors.Add("Error with Operation in Line: " + pCurrent.lineNo);
                }
            }

            // if definition
            else if (pCurrent.token == "INT" || pCurrent.token == "STR"){
                //moves to next index and lexeme
                if (pCurrent.lexeme != "LOI") {
                    pIndex++;
                    pCurrent = lexemesParse[pIndex];
                }

                if (pCurrent.token == "VAR_NAME")
                {
                    //moves to next index and lexeme
                    if (pCurrent.lexeme != "LOI") {
                        pIndex++;
                        pCurrent = lexemesParse[pIndex];
                    }

                    if (pCurrent.token == "IS") {
                        //moves to next index and lexeme
                        if (pCurrent.lexeme != "LOI"){
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                        }
                        if (pCurrent.token == "INT_LIT" || pCurrent.token == "VAR_NAME") {
                            return;
                        }
                    } else {
                        return;
                    }
                }
                Errors.Add("Error with Definition in Line: " + pCurrent.lineNo);
            } else if (pCurrent.token == "NEWLN"){
                //do nothing
            }
        }

        //helper function for parsing
        // for statements containing arithmetic keywords
        private void Arith() {
            if (pCurrent.token == "INT_LIT" || pCurrent.token == "VAR_NAME"){
                return;
            }

            else if (artithmeticKeyword.Contains(pCurrent.token)){
                //moves to next index and lexeme
                if (pCurrent.lexeme != "LOI"){
                    pIndex++;
                    pCurrent = lexemesParse[pIndex];
                }
                Arith();
                //moves to next index and lexeme
                if (pCurrent.lexeme != "LOI") {
                    pIndex++;
                    pCurrent = lexemesParse[pIndex];
                }
                Arith();
            }else {
                Errors.Add("Error with Expression in Line: " + pCurrent.lineNo);
            }
        }

        // Event button for Showing tokenized code
        
        private void showTokenizedToolStripMenuItem_Click(object sender, EventArgs e) {
            richTextBox2.Clear();
            int index = 1;
            //richTextBox2.Text += lexemesParse.Count();
           

            for (int i = 1; i < rawInput.Length - 1; i++)  {
                // removes all whitespaces before and after lines inputs
                string input = rawInput[i].Trim();
                string[] inputLine = input.Split(' ');
                foreach (string item in inputLine)  {
                    richTextBox2.Text = richTextBox2.Text + "<" +lexemesParse[index].token + "> ";
                    index++;
                }
                richTextBox2.Text = richTextBox2.Text + "\n";
            }
            showTokenizedToolStripMenuItem.Enabled = true;

            string itsthefile = Path.GetFileName(filePath);
            string filenamenoext = Path.ChangeExtension(itsthefile, null);
            filenamenoext = filenamenoext + ".tkn";

            //save tokenized code automatically as tkn file
            // Check if file already exists. If yes, delete it.     
            if (File.Exists(filenamenoext)) {
                    File.Delete(filenamenoext);
                }

                // Create a new file     
                using (FileStream fs = File.Create(filenamenoext)) {
                // Add text to file
                    string tokenized = richTextBox2.Text;                 
                    byte[] tokenizedB = new UTF8Encoding(true).GetBytes(tokenized);
                    fs.Write(tokenizedB, 0, tokenizedB.Length);
                }
        }

        //holds value of variable used for execution
         Dictionary<string, string> varVal = new Dictionary<string, string>();
        string exeResult; //a string that will store all execution results
        private void executeToolStripMenuItem_Click(object sender, EventArgs e)  {
            richTextBox2.Text += "\nProgram test will now be executed…\n";
            //reinitialize pointers
            pIndex = 1;
            pCurrent = lexemesParse[pIndex];
            // richTextBox2.Clear();
            exeResult = "";
            varVal.Clear();

            while (pCurrent.lexeme != "LOI")

                //INTO, BEG PRINT
                if (pCurrent.token == "INTO" || pCurrent.token == "BEG" || pCurrent.token == "PRINT")
                {
                    // richTextBox2.Text += "infinite ";

                    switch (pCurrent.token)
                    {
                        case "INTO":
                            executeINTO();
                            //moves to next index and lexeme
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                            //if (pCurrent.lexeme != "LOI") return;
                            break;

                        case "BEG":
                            //skip identifier moves to next
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                            // if (pCurrent.lexeme != "LOI") return;

                            richTextBox2.Text += "\nInput for " + lexemesParse[pIndex].lexeme + ": ";
                            //asks for user input for variable value assignment
                            string temp = pCurrent.lexeme;
                            Form2 user = new Form2();
                            user.ShowDialog();

                            varVal[temp] = Form2.userInput;
                            richTextBox2.Text += varVal[temp];
                            //moves to next index and lexeme
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                            // if (pCurrent.lexeme != "LOI") return;
                            break;

                        case "PRINT":
                            //moves to next index and lexeme
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];

                            //if followed by arithmetic operation
                            if (artithmeticKeyword.Contains(lexemesParse[pIndex].token))
                            {
                                int item = 0;

                                switch (pCurrent.token)
                                {
                                    case "ADD":
                                        item = excuteArithmetic(1);
                                        //moves to next index and lexeme
                                        pIndex++;
                                        pCurrent = lexemesParse[pIndex];
                                        // if (pCurrent.lexeme != "LOI") return;
                                        break;

                                    case "SUB":
                                        item = excuteArithmetic(2);
                                        //moves to next index and lexeme
                                        pIndex++;
                                        pCurrent = lexemesParse[pIndex];
                                        //  if (pCurrent.lexeme != "LOI") return;
                                        break;

                                    case "MULT":
                                        item = excuteArithmetic(3);
                                        //moves to next index and lexeme
                                        pIndex++;
                                        pCurrent = lexemesParse[pIndex];
                                        //if (pCurrent.lexeme != "LOI") return;
                                        break;

                                    case "DIV":
                                        item = excuteArithmetic(4);
                                        //moves to next index and lexeme
                                        pIndex++;
                                        pCurrent = lexemesParse[pIndex];
                                        // if (pCurrent.lexeme != "LOI") return;
                                        break;

                                    case "MOD":
                                        item = excuteArithmetic(5);
                                        //moves to next index and lexeme
                                        pIndex++;
                                        pCurrent = lexemesParse[pIndex];
                                        //if (pCurrent.lexeme != "LOI") return;
                                        break;
                                }
                                richTextBox2.Text += "\n" + item;
                            }
                            else//if followed by variable
                            {
                                string t = pCurrent.lexeme;
                                string v;
                                varVal.TryGetValue(t, out v);

                                richTextBox2.Text += "\n" + v;
                                //exeResult += "\n" + v;

                                //moves to next index and lexeme
                                pIndex++;
                                pCurrent = lexemesParse[pIndex];
                                // if (pCurrent.lexeme != "LOI") return;
                            }

                            break;
                    }
                }

                //For Arithmetic Operations
                else if (artithmeticKeyword.Contains(pCurrent.token))
                {
                    int item;
                    switch (pCurrent.token)
                    {
                        case "ADD":
                            item = excuteArithmetic(1);
                            //moves to next index and lexeme
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                            // if (pCurrent.lexeme != "LOI") return;
                            break;

                        case "SUB":
                            item = excuteArithmetic(2);
                            //moves to next index and lexeme
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                            //  if (pCurrent.lexeme != "LOI") return;
                            break;

                        case "MULT":
                            item = excuteArithmetic(3);
                            //moves to next index and lexeme
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                            //if (pCurrent.lexeme != "LOI") return;
                            break;

                        case "DIV":
                            item = excuteArithmetic(4);
                            //moves to next index and lexeme
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                            // if (pCurrent.lexeme != "LOI") return;
                            break;

                        case "MOD":
                            item = excuteArithmetic(5);
                            //moves to next index and lexeme
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                            //if (pCurrent.lexeme != "LOI") return;
                            break;
                    }
                }

                //data type declaration
                else if (pCurrent.token == "STR" || pCurrent.token == "INT")
                {
                    if (pCurrent.token == "STR")
                    {
                        //skip identifier moves to next
                        pIndex++;
                        pCurrent = lexemesParse[pIndex];
                        // if (pCurrent.lexeme != "LOI") return;

                        //saves value to dictionary for current lexeme
                        varVal.Add(pCurrent.lexeme, "");

                        //moves to next index and lexeme
                        pIndex++;
                        pCurrent = lexemesParse[pIndex];
                        // if (pCurrent.lexeme != "LOI") return;
                    }
                    else
                    {//if followed by variable name
                     //skip identifier moves to next
                        pIndex++;
                        pCurrent = lexemesParse[pIndex];
                        // if (pCurrent.lexeme != "LOI") return;

                        string z = pCurrent.lexeme;

                        //moves to next index and lexeme
                        pIndex++;
                        pCurrent = lexemesParse[pIndex];
                        // if (pCurrent.lexeme != "LOI") return;


                        if (pCurrent.token == "IS")
                        {
                            //skip INT_LIT moves to next
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                            // if (pCurrent.lexeme != "LOI") return;

                            varVal[z] = pCurrent.lexeme;

                            //moves to next index and lexeme
                            pIndex++;
                            pCurrent = lexemesParse[pIndex];
                            //if (pCurrent.lexeme != "LOI") return;
                        }
                    }
                }
                else if (pCurrent.token == "NEWLN")
                {
                    //skip NEWLN moves to next
                    pIndex++;
                    pCurrent = lexemesParse[pIndex];
                    //  if (pCurrent.lexeme != "LOI") return;
                }
             
            richTextBox2.Text += "\n\n Program terminated successfully...";
            return;
        }//end of exe main function

        //helper function for INTO

        private void executeINTO()  {

            int arith = 0; //assignment for arith cases

            //skip identifier moves to next
            pIndex++;
            pCurrent = lexemesParse[pIndex];
          //  if (pCurrent.lexeme != "LOI") return;

            string temp = pCurrent.lexeme;

            //skip IS moves to next
            pIndex++;
            pCurrent = lexemesParse[pIndex];
           // if (pCurrent.lexeme != "LOI") return;

            //skip literal moves to next
            pIndex++;
            pCurrent = lexemesParse[pIndex];
            //if (pCurrent.lexeme != "LOI") return;

            //if integer, save to variable value dictionary
            if (pCurrent.token == "INT_LIT") {
                varVal[temp] = pCurrent.lexeme;
            } else{//if arithmetic operation forward to arithmetic helper function
                if (pCurrent.lexeme == "ADD")
                { arith = 1; }
                else if (pCurrent.lexeme == "SUB")
                { arith = 2; }
                else if (pCurrent.lexeme == "MULT")
                { arith = 3; }
                else if (pCurrent.lexeme == "DIV")
                { arith = 4; }
                else if (pCurrent.lexeme == "MOD")
                { arith = 5; }

                int ans = excuteArithmetic(arith);
                varVal[temp] = Convert.ToString(ans);
            }
            return;
        }

        private int excuteArithmetic(int arith)
        {
            int result = 0;

            //skip literal/variable moves to next
            pIndex++;
            pCurrent = lexemesParse[pIndex];

            int num1 = 0;
            int num2 = 0;

            //if num 1 integer literal
            if (pCurrent.token == "INT_LIT") {
                //convert string to integer
                num1 = Int32.Parse(pCurrent.lexeme);

                //move to num2
                pIndex++;
                pCurrent = lexemesParse[pIndex];
               

                //if num2 integer literal
                if (pCurrent.token == "INT_LIT") {
                    num2 = Int32.Parse(pCurrent.lexeme);
                }
                else { //if num2 is a variable
                    string temp;
                    varVal.TryGetValue(pCurrent.lexeme, out temp); //get value from variable table
                    num2 = Int32.Parse(temp);//convert string to integer
                }
            } else { // num 1 is avariable
                string temp;
                varVal.TryGetValue(pCurrent.lexeme, out temp);//get value from variable table
                num1 = Int32.Parse(temp); ;//convert string to integer

                //move to num2
                pIndex++;
                pCurrent = lexemesParse[pIndex];
               

                if (pCurrent.token == "INT_LIT")  //if num2 integer literal
                    num2 = Int32.Parse(pCurrent.lexeme);
                else {//if num2 is a variable
                    string temp1;
                    varVal.TryGetValue(pCurrent.lexeme, out temp1); //get value from variable table
                    num2 = Int32.Parse(temp1);//convert string to integer
                }
            }

            //ADD
            if (arith == 1)result = num1 + num2;
            //sub
            else if (arith == 2) result = num1 - num2;
            //mult
            else if (arith == 3) result = num1 * num2;
            //div
            else if (arith == 4) result = num1 / num2;
            //mod
            else if (arith == 5) result = num1 % num2;

           // richTextBox2.Text = "\n answer: " + result;
            return result;
        }


    }   //end of Form1 class   

    //class for word for easier storing of lexemes for parsing
    public class Word
    {

        public string lexeme { get; set; }
        public string token { get; set; }
        public int lineNo { get; set; }

        public Word(string lex, int lnNo)
        {
            this.lexeme = lex;
            this.token = string.Empty;
            this.lineNo = lnNo;

        }
    }
}
