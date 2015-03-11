using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
    public class PostingBeforeProcessor
    {
        public string ProcessedText { get; private set; }
        public string BaseText { get; set; }
        public PostingBeforeProcessor(string writeText)
        {
            BaseText = ProcessedText = writeText;
        }

        public void ReplaceEndOfLine()
        {
            ProcessedText = BaseText.Replace("\r\n", "  ");
        }
    }
}
