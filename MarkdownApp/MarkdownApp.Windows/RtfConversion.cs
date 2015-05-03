using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;

namespace MarkdownApp
{
    class RtfConversion
    {
        public static void ToHtml(ITextDocument doc)
        {
            // Takes a RichEditBox control and returns a
            // simple HTML-formatted version of its contents
            string strHTML, strFont, strColour, strBold, strFntName;
            float shtSize;
            int lngOriginalStart, lngOriginalLength;
            int intCount = 0;
            ITextRange tr = doc.GetRange(0, 9999999);

            // If nothing in the box, exit
            if (tr.Length == 0)
            {
                //App.Current.Exit();
            }
            
            // Store original selections, then select first character
            lngOriginalStart = 0;
            lngOriginalLength = tr.Length;
            tr.SetRange(0, 1);
            
            // Add HTML header
            strHTML = "<html>";
            
            // Set up initial parameters
            strColour = tr.CharacterFormat.ForegroundColor.ToString();
            strFont = tr.CharacterFormat.FontStyle.ToString();
            shtSize = tr.CharacterFormat.Size;
            strBold = tr.CharacterFormat.Bold.ToString();
            strFntName = tr.CharacterFormat.Name;
            Debug.WriteLine("Colour: " + strColour);
            
            // Include first 'style' parameters in the HTML
            strHTML += "<span style=\"font-family:" + strFntName + "; font-size: " + shtSize + "pt; color: #" + strColour.Substring(3) + "\">";
            
            // Include bold tag, if required
            if (strBold.ToLower() == "on")
                strHTML += "<b>";
            // Include italic tag, if required
            if (strFont.ToLower() == "italic")
                strHTML += "<i>";
            
            // Finally, add our first character
            strHTML += tr.Character;
            
            // Loop around all remaining characters
            for (intCount = 2; intCount < lngOriginalLength; intCount++)
            {
                // Select current character
                tr.SetRange(intCount - 1, intCount + 1);
                
                //     If this is a line break, add HTML tag
                if (tr.Character == Convert.ToChar(13))
                    strHTML += "<br />";
                
                //    ' Check/implement any changes in style
                if (tr.CharacterFormat.ForegroundColor.ToString() != strColour || tr.CharacterFormat.Name != strFntName || tr.CharacterFormat.Size != shtSize)
                {
                    strHTML += "</span><span style=\"font-family: " + tr.CharacterFormat.Name + "; font size: " + tr.CharacterFormat.Size + "pt; color: #" + tr.CharacterFormat.ForegroundColor.ToString().Substring(3) + "\">";
                }
                
                //     Check for bold changes
                if (tr.CharacterFormat.Bold.ToString().ToLower() != strBold.ToLower())
                {
                    if (tr.CharacterFormat.Bold.ToString().ToLower() != "on")
                        strHTML += "</b>";
                    else
                        strHTML += "<b>";
                }
                
                //    Check for italic changes
                if (tr.CharacterFormat.FontStyle.ToString().ToLower() != strFont.ToLower())
                {
                    if (tr.CharacterFormat.FontStyle.ToString().ToLower() != "italic")
                        strHTML += "</i>";
                    else
                        strHTML += "<i>";
                }
                
                //    ' Add the actual character
                strHTML += tr.Character;
                
                //    ' Update variables with current style
                strColour = tr.CharacterFormat.ForegroundColor.ToString();
                strFont = tr.CharacterFormat.FontStyle.ToString();
                shtSize = tr.CharacterFormat.Size;
                strFntName = tr.CharacterFormat.Name;
                strBold = tr.CharacterFormat.Bold.ToString();
            }
            
            // Close off any open bold/italic tags
            if (strBold == "on")
                strHTML += "";
            if (strFont.ToLower() == "italic")
                strHTML += "";
            
            // Terminate outstanding HTML tags
            strHTML += "</span></html>";
            
            //' Restore original RichTextBox selection
            tr.SetRange(lngOriginalStart, lngOriginalLength);

            doc.SetText(TextSetOptions.FormatRtf, strHTML);
        }
    }
}
