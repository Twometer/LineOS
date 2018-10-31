using System;

namespace LineOS.CLI
{
    public class TablePrinter
    {
        private int[] offsets;

        private const string Space = "    ";

        public void WriteHeaders(params string[] headers)
        {
            offsets = new int[headers.Length];
            var offset = 4;

            Console.Write(Space);
            for (var i = 0; i < headers.Length; i++)
            {
                var header = headers[i];
                offsets[i] = offset;

                var content = header + Space;
                Console.Write(content);
                offset += content.Length;
            }
            Console.WriteLine();

            foreach (var header in headers)
                Console.Write(Space + new string('=', header.Length));
            Console.WriteLine();
        }

        public void WriteRow(params string[] cols)
        {
            var textLength = 0;
            for(var i = 0; i < cols.Length; i++)
            {
                var col = cols[i];
                var off = new string(' ', offsets[i] - textLength);
                Console.Write(off);
                Console.Write(col);
                textLength += col.Length + off.Length;
            }
            Console.WriteLine();
        }

    }
}
