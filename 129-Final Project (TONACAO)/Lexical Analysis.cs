using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _129_Final_Project__TONACAO_
{
    class LexicalAnalysis
    {
        string newline = Environment.NewLine;
        List<string> Keywords = new List<string>();
        List<int> Int_Literals = new List<int>();
        List<string> Identifiers = new List<string>();
        List<string> Errors = new List<string>();
        List<string> IOL = new List<string>();
        List<string> LOI = new List<string>();


        private int getNextCharacter(char cChar, int type)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s]*$");
           
            string cString = cChar.ToString(); //converts the character to a string literal for regex comparison
            if (cChar == ' ')
            {
                //if whitespace encountered  is preceeded with a character that is not a lexeme
                if (type == 3)
                {
                    return 6;
                }
                //if a starting whitespace
                if (type == 0)
                {
                    return 0;
                }
                //if whitespace encountered and type determined is an identifier 
                else if (type == 1)
                {
                    return 2;
                }//if whitespace encountered is preceeded wih an int literal
                else if (type == 2)
                {
                    return 4;
                }
            }
            //if character is a letter
            else if (char.IsLetter(cChar))
            {
                return 1;
            }
            //if character is an integer
            else if (char.IsDigit(cChar))
            {
                return 3;
            }
            //if character is invalid and not a lexeme
            else if (!rg.IsMatch(cString))
            {
                return 5;
            }
            //if character is \n
            return 7;
        }

        //helper function to check if string read is a valid keyword
        private bool isKeyword(string sToken)
        {
            if ((sToken).Length > 16 || (sToken).Length == 0)
                return false;
            var sKeywords = new List<string>(){
                "INTO", "IS", "INT", "STR", "BEG", "PRINT", "NEWLN", "ADD", "SUB", "MULT", "DIV", "MOD"}; //declares valid keywords
            return sKeywords.Exists(element => (sToken) == element);//checks if string exist as valid keyword
        }
        //helper function to check if string read is a valid keyword (LOI,IOL)
        private bool isCoding(string sToken)
        {
            if ((sToken).Length > 3 || (sToken).Length == 0) //invalidates string longer than 3 characters
                return false;
            var sKeywords = new List<string>() { "IOL", "LOI" }; //declares valid keywords
            return sKeywords.Exists(element => (sToken) == element);//checks if string exist as valid keyword
        }

        string resultStatus, result;

        public string ShowTokenized(string txt)
        {
            Result(txt);
            return result;
        }

            //main function
        public string Result(string txt)
        {
            //if  empty
            if (txt.Length == 0)
                return "";

            //initialize result
            //var result = "";

            //declare valid string input
            Regex rg = new Regex(@"^[a-zA-Z0-9\s]*$");

            //initialize variables
            int txtIndex = 0, iState = 0, type = 0, endpointer = 0, startpointer = 1, currentpointer = 0, linepointer = 0,
                errorcount = 0, errorstore = 0;

            //Type 0 = Whitespace, Type 1 = Char, Type 2 = Digit, Type 3 = Invalid
            char cChar;
            string sToken = "";//string of token
            var errorlines = "";
            txt += " ";

            //scan character by character
            while (txtIndex < txt.Length)
            {
                //take single character from string
                cChar = txt[txtIndex];
                txtIndex++;

                //classifies type of character
                iState = getNextCharacter(cChar, type);
                switch (iState)
                {
                    //Whitespace
                    case 0:
                        iState = 0;
                        type = 0;
                        sToken = "";
                        break;
                    //reads a character but word/sToken not yet complete
                    case 1:
                        //add character to string of token
                        sToken += cChar;
                        type = 1;
                        break;
                    //whitespace encountered end of word/string of token
                    case 2:
                        if (isKeyword(sToken))//checks if token is valid
                        {
                            result += "<" + sToken + ">"; //append in token list
                            if (!Keywords.Contains(sToken))
                                Keywords.Add(sToken);
                        }
                        else if (isCoding(sToken))//check if token is IOL/LOI
                        {
                            if (sToken == "IOL")
                            {
                                result += "<" + sToken + ">";
                                IOL.Add(sToken);
                                startpointer = currentpointer;
                            }
                            else if (sToken == "LOI")
                            {
                                result += "<" + sToken + ">";
                                LOI.Add(sToken);
                                endpointer = currentpointer;
                            }
                        }
                        else
                        {//if error lexeme
                            if (char.IsDigit(sToken[0]))
                            {
                                errorcount++;
                                result += "<ERR_LEX>";
                                if (!Errors.Contains(sToken))
                                    Errors.Add(sToken);
                            }
                            else// identifier
                            {
                                result += "<IDENT>";
                                if (!Identifiers.Contains(sToken))
                                    Identifiers.Add(sToken);
                            }
                        }
                        currentpointer++;
                        iState = 0;
                        type = 0;
                        sToken = "";
                        break;
                    case 3:  //reads a int literal but word/sToken not yet complete
                        sToken += cChar;
                        type = 2;
                        break;
                    case 4://  //reads complete word with last scannned int literal 
                        if (char.IsLetter(sToken[0])) //if entire word not an int literal
                        {
                            result += "<IDENT>"; //append token as identifier
                            if (!Identifiers.Contains(sToken))
                                Identifiers.Add(sToken);
                        }
                        else //if entire word is int literal
                        {
                            result += "<INT_LIT, " + sToken + ">";//append token as int liteal
                            if (!Int_Literals.Contains(int.Parse(sToken)))
                                Int_Literals.Add(int.Parse(sToken));
                        }
                        currentpointer++;
                        iState = 0;
                        type = 0;
                        sToken = "";
                        break;
                    case 5:// invalid character  but word/sToken not yet complete 
                        sToken += cChar;
                        type = 3;
                        break;
                    case 6:// all error lexemes 
                        result += "<ERR_LEX>";
                        sToken += cChar;
                        if (!Errors.Contains(sToken))
                            Errors.Add(sToken);
                        currentpointer++;
                        errorcount++;
                        iState = 0;
                        type = 0;
                        sToken = "";
                        break;
                    case 7://reads \n proceeds to conduct all appropriate processes
                        if (!rg.IsMatch(sToken))
                        {
                            errorcount++;
                            result += "<ERR_LEX>";
                            if (!Errors.Contains(sToken))
                                Errors.Add(sToken);
                            if (errorcount > 0)
                            {
                                errorlines += "Errors found in line " + (linepointer + 1);
                                for (int i = errorstore; i < Errors.Count; i++)
                                {
                                    errorlines += " " + Errors[i];
                                }
                                errorlines += newline;
                                errorstore += errorcount;
                            }
                            currentpointer++;
                            linepointer++;
                            iState = 0;
                            type = 0;
                            errorcount = 0;
                            sToken = "";
                            result += newline;
                            break;
                        }
                        if (type == 1)
                        {
                            if (isKeyword(sToken))
                            {
                                result += "<" + sToken + ">";
                                if (!Keywords.Contains(sToken))
                                    Keywords.Add(sToken);
                            }
                            else if (isCoding(sToken))
                            {
                                if (sToken == "IOL")
                                {
                                    result += "<" + sToken + ">";
                                    IOL.Add(sToken);
                                    startpointer = currentpointer;
                                }
                                else if (sToken == "LOI")
                                {
                                    result += "<" + sToken + ">";
                                    LOI.Add(sToken);
                                    endpointer = currentpointer;
                                }
                            }
                            else
                            {
                                if (char.IsDigit(sToken[0]))
                                {
                                    errorcount++;
                                    result += "<ERR_LEX>";
                                    if (!Errors.Contains(sToken))
                                        Errors.Add(sToken);
                                }
                                else
                                {
                                    result += "<IDENT>";
                                    if (!Identifiers.Contains(sToken))
                                        Identifiers.Add(sToken);
                                }
                            }
                        }
                        else if (type == 2)
                        {
                            if (char.IsLetter(sToken[0]))
                            {
                                result += "<IDENT>";
                                if (!Identifiers.Contains(sToken))
                                    Identifiers.Add(sToken);
                            }
                            else
                            {
                                result += "<INT_LIT, " + sToken + ">";
                                if (!Int_Literals.Contains(int.Parse(sToken)))
                                    Int_Literals.Add(int.Parse(sToken));
                            }
                        }
                        if (errorcount > 0)
                        {
                            errorlines += "Errors found in line " + (linepointer + 1);
                            for (int i = errorstore; i < Errors.Count; i++)
                            {
                                errorlines += " " + Errors[i];
                            }
                            errorlines += newline;
                            errorstore += errorcount;
                        }
                        currentpointer++;
                        linepointer++;
                        errorcount = 0;
                        iState = 0;
                        type = 0;
                        sToken = "";
                        result += newline;
                        break;
                }
            }
            //append new line
            result += newline;

            //create and writes result in output file
            //File.WriteAllTextAsync("Output.tkn", result);

            //append all scanned keywords in result
            result += "Keywords: ";
            for (int i = 0; i < Keywords.Count; i++)
            {
                if (i == Keywords.Count - 1)
                    result += Keywords[i];
                else
                    result += Keywords[i] + ", ";
            }

            //append all int literals in result
            result += newline;
            result += "Integer literals: ";
            for (int i = 0; i < Int_Literals.Count; i++)
            {
                if (i == Int_Literals.Count - 1)
                    result += Int_Literals[i];
                else
                    result += Int_Literals[i] + ", ";
            }
            result += newline;

            //append all scanned identifiers in result
            result += "Identifiers: ";
            for (int i = 0; i < Identifiers.Count; i++)
            {
                if (i == Identifiers.Count - 1)
                    result += Identifiers[i];
                else
                    result += Identifiers[i] + ", ";
            }
            result += newline;

            //append format error info in result
            if (startpointer != 0 || endpointer != currentpointer - 1)
            {
                resultStatus += "Invalid format of IOL file" + newline;
            }
            //append lexical errors info in result
            if (Errors.Count > 0)
            {
                resultStatus += "Error found in performing Lexical Analysis" + newline;
                resultStatus += errorlines + newline;
                resultStatus += "Errors: ";
                for (int i = 0; i < Errors.Count; i++)
                {
                    if (i == Errors.Count - 1)
                        result += Errors[i];
                    else
                        result += Errors[i] + ", ";
                }
            }
            if (startpointer == 0 && endpointer == currentpointer - 1 && Errors.Count == 0)
            {
                //result += "Lexical Analysis Successfully Ran";
                resultStatus += "Lexical Analysis Successfully Ran";
            }

            //return result;
            return resultStatus;//returns string of lexical analysis result
        }
    }
}
