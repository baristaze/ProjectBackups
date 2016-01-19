using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crosspl.ObjectModel
{
    public class SimpleTagReplacer
    {
        public char TagChar { get; private set; }
        public string ReplaceOpen { get; private set; }
        public string ReplaceClose { get; private set; }

        public SimpleTagReplacer(char tagChar, string replaceOpen, string replaceClose)
        { 
            this.TagChar  = tagChar;
            this.ReplaceOpen = replaceOpen;
            this.ReplaceClose  = replaceClose;
        }

        public string MatchAndReplaceAll(string text)
        {
            bool continueToProcess = true;
            while (continueToProcess)
            {
                string altered = this.MatchNextAndReplace(text);
                if (altered != text)
                {
                    text = altered;
                }
                else
                {
                    continueToProcess = false;
                }
            }

            return text;
        }

        public string MatchNextAndReplace(string text)
        {
            int previous = -1;
            string tag = "" + this.TagChar + this.TagChar;
            bool isEscaped = false;
            for (int x = 0; x < text.Length; x++)
            {
                if (text[x] == '\\')
                {
                    isEscaped = true;
                }
                else if (isEscaped == true && text[x] == this.TagChar)
                {
                    // skip... i.e. do nothing
                }
                else
                {
                    isEscaped = false;
                }

                if (!isEscaped && x >= 1)
                {

                    string sub = text.Substring(x - 1, 2); // x+1 is not included // if [ab] => we are on 'b'
                    if (sub == tag)
                    {
                        if (previous < 0)
                        {
                            previous = x - 1; // do not include this, since x-1 = * and x = *
                        }
                        else
                        {
                            string pre = text.Substring(0, previous); // [this is ]**bold** and // previous is not included
                            string middle = text.Substring(previous + 2, x - previous - 3); // x-1 = * // do not include x-1
                            string post = "";
                            if (x != text.Length - 1)
                            {
                                post = text.Substring(x + 1, text.Length - x - 1); // text.length is not included
                            }

                            string altered = pre + this.ReplaceOpen + middle + this.ReplaceClose + post;
                            return altered;
                        }
                    }
                }
            }

            return text;
        }
    }
}
