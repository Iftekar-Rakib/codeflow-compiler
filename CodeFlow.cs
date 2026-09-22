using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeFlow.CompilerBeta;


namespace CodeFlow
{
    public partial class CompilerBeta : System.Windows.Forms.Form
    {
        public string sourceProgram;
        public int currentPointer;
        public token currentToken;
        public IdentifierTable Identifiers;
        public TemporaryVariableTable tempVars;
        public QuadrupleTable midCodes;

        private void print(string s)
        {
            listBox.Items.Add(s);
        }
        private void printToken(token t)
        {
            listBox.Items.Add($"[{t.type}, {t.value}]");
        }
        private void buttonTokenize_Click(object sender, EventArgs e)
        {
            sourceProgram = textBox1.Text + "#";
            listBox.Items.Clear();
            listBox1.Items.Clear();
            print(sourceProgram);
            currentPointer = 0;// Get the first token
            currentToken = tokenizer();// Print the first token using printToken method
            printToken(currentToken);
            // Loop to process all tokens until the end-of-input marker
            while (currentToken.type != "#")
            {
                currentToken = tokenizer();  // Get the next token
                printToken(currentToken);  // Print each token within square brackets
            }
            listBox.Items.Add("end..");// Indicate the end of tokenization
        }


        public CompilerBeta()
        {
            InitializeComponent();
            this.KeyPreview = true;
            listBox.KeyDown += new KeyEventHandler(listBox_KeyDown);
            listBox1.KeyDown += new KeyEventHandler(listBox1_KeyDown);

            this.Text = "CodeFlow Compiler";
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            textBox1.BackColor = System.Drawing.Color.FromArgb(15, 15, 40);
            textBox1.ForeColor = System.Drawing.Color.Cyan;
            textBox1.Font = new System.Drawing.Font("Consolas", 11F);

            button1.Text = "⚡ Tokenize";
            button1.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            button1.ForeColor = System.Drawing.Color.White;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.FlatAppearance.BorderColor = System.Drawing.Color.Cyan;
            button1.FlatAppearance.BorderSize = 2;

            buttonParse.Text = "🚀 Parse";
            buttonParse.BackColor = System.Drawing.Color.FromArgb(0, 180, 100);
            buttonParse.ForeColor = System.Drawing.Color.White;
            buttonParse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonParse.FlatAppearance.BorderColor = System.Drawing.Color.LimeGreen;
            buttonParse.FlatAppearance.BorderSize = 2;

            listBox.BackColor = System.Drawing.Color.FromArgb(10, 10, 30);
            listBox.ForeColor = System.Drawing.Color.LightGreen;
            listBox.Font = new System.Drawing.Font("Consolas", 10F);

            listBox1.BackColor = System.Drawing.Color.FromArgb(10, 10, 30);
            listBox1.ForeColor = System.Drawing.Color.Orange;
            listBox1.Font = new System.Drawing.Font("Consolas", 10F);
        }
        private void CopyAllItemsToClipboard(ListBox listBox)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in listBox.Items)
            {
                sb.AppendLine(item.ToString());
            }
            Clipboard.SetText(sb.ToString());
        }
        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyAllItemsToClipboard(listBox);
            }
        }
        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyAllItemsToClipboard(listBox1);
            }
        }


        public class token
        {
            public string type { get; set; }
            public string value { get; set; }
            public token(string t, string v)
            {
                this.type = t;
                this.value = v;
            }

            public override string ToString()
            {
                return string.Concat(new string[]
                {
                    "(",
                    this.type,
                    ",",
                    this.value,
                    ")"
                });
            }
        }


        private token tokenizer()
        {
            string word = "";

            while (currentPointer < sourceProgram.Length)
            {
                char ch = sourceProgram[currentPointer];

                // Skip ALL whitespace (better and cleaner)
                if (char.IsWhiteSpace(ch))
                {
                    currentPointer++;
                    continue;
                }

                // 🔹 IDENTIFIER starting with $
                if (ch == '$')
                {
                    word += ch;
                    currentPointer++;

                    while (currentPointer < sourceProgram.Length)
                    {
                        ch = sourceProgram[currentPointer];
                        if (char.IsLetterOrDigit(ch))
                        {
                            word += ch;
                            currentPointer++;
                        }
                        else break;
                    }

                    return new token("identifier", word);
                }

                // 🔹 KEYWORDS or identifiers
                if (char.IsLetter(ch))
                {
                    word += ch;
                    currentPointer++;

                    while (currentPointer < sourceProgram.Length)
                    {
                        ch = sourceProgram[currentPointer];

                        // 🔥 FIX: stop when '$' appears → fixes int$f
                        if (ch == '$') break;

                        if (char.IsLetterOrDigit(ch))
                        {
                            word += ch;
                            currentPointer++;
                        }
                        else break;
                    }

                    if (word == "int") return new token("kw_int", word);
                    if (word == "if") return new token("kw_if", word);
                    if (word == "while") return new token("kw_while", word);
                    if (word == "do") return new token("kw_do", word);
                    if (word == "then") return new token("kw_then", word);
                    if (word == "else") return new token("kw_else", word);
                    if (word == "begin") return new token("kw_begin", word);
                    if (word == "end") return new token("kw_end", word);

                    return new token("identifier", word);
                }

                // 🔹 NUMBERS
                if (char.IsDigit(ch))
                {
                    word += ch;
                    currentPointer++;

                    while (currentPointer < sourceProgram.Length)
                    {
                        ch = sourceProgram[currentPointer];
                        if (char.IsDigit(ch))
                        {
                            word += ch;
                            currentPointer++;
                        }
                        else break;
                    }

                    return new token("integer", word);
                }

                // 🔹 OPERATORS
                if (ch == '+')
                {
                    currentPointer++;
                    return new token("opPlus", "+");
                }

                if (ch == '*')
                {
                    currentPointer++;
                    return new token("opTime", "*");
                }

                if (ch == '=')
                {
                    currentPointer++;
                    return new token("assign", "=");
                }

                if (ch == '>')
                {
                    currentPointer++;
                    if (currentPointer < sourceProgram.Length && sourceProgram[currentPointer] == '=')
                    {
                        currentPointer++;
                        return new token("log_op", ">=");
                    }
                    return new token("log_op", ">");
                }

                if (ch == '<')
                {
                    currentPointer++;
                    return new token("log_op", "<");
                }

                // 🔹 SYMBOLS
                if (ch == ';')
                {
                    currentPointer++;
                    return new token("semiColon", ";");
                }

                if (ch == ',')
                {
                    currentPointer++;
                    return new token("comma", ",");
                }

                if (ch == '(')
                {
                    currentPointer++;
                    return new token("leftP", "(");
                }
                if (ch == '#')
                {
                    currentPointer++;
                    return new token("#", "#");
                }

                if (ch == ')')
                {
                    currentPointer++;
                    return new token("rightP", ")");
                }

                // ❌ Unknown character
                currentPointer++;
                return new token("unknown", ch.ToString());
            }

            return new token("#", "#");
        }



        public class Identifier
        {
            public string name { get; set; }
            public string type { get; set; }
            public string value { get; set; }

            public Identifier(string n)
            {
                this.name = n;
                this.type = "";
                this.value = "";
            }

            public override string ToString()
            {
                return $" {this.name} | [{this.type}] | [{this.value}]";
            }
        }

        public class IdentifierTable
        {
            private List<Identifier> list;

            public IdentifierTable()
            {
                list = new List<Identifier>();
            }


            public Identifier getIdentifierByName(string name)
            {
                foreach (Identifier t in list)
                {
                    if (t.name == name) return t;
                }
                return null;
            }

            public Boolean add(string name)
            {
                Identifier t = getIdentifierByName(name);
                if (t != null) return false;
                list.Add(new Identifier(name));
                return true;
            }

            public Boolean updateTypeByName(string name, string type)
            {
                Identifier t = getIdentifierByName(name);
                if (t == null) return false;
                t.type = type;
                return true;
            }

            public Boolean updateValueByName(string name, string value)
            {
                Identifier t = getIdentifierByName(name);
                if (t == null) return false;
                t.value = value;
                return true;
            }

            public void dump(ListBox lb)
            {
                lb.Items.Add("<--------- Identifier Table --------->");
                lb.Items.Add(string.Format("{0,-12} | {1,-10} | {2,-10}", "Name", "Type", "Value"));
                lb.Items.Add(new string('-', 36));
                foreach (Identifier t in list)
                {
                    lb.Items.Add(string.Format("{0,-12} | {1,-10} | {2,-10}", t.name, t.type, t.value));
                }
                lb.Items.Add(new string('-', 36));
            }
        }

        public class TemporaryVariableTable
        {
            private List<Identifier> list;

            public TemporaryVariableTable()
            {
                list = new List<Identifier>();
            }

            public Identifier CreateNewTempVar()
            {
                int index = list.Count;
                Identifier t = new Identifier($"T{index}");
                list.Add(t);
                return t;
            }


            public void dump(ListBox lb)
            {
                lb.Items.Add("<---- Temp Variable Table ---------->");
                lb.Items.Add(string.Format("{0,-12} | {1,-10} | {2,-10}", "Name", "Type", "Value"));
                lb.Items.Add(new string('-', 36));
                foreach (Identifier t in list)
                {
                    lb.Items.Add(string.Format("{0,-12} | {1,-10} | {2,-10}", t.name, t.type, t.value));
                }
                lb.Items.Add(new string('-', 36));
            }

        }

        public class Quadruple
        {
            public string Op1 { get; set; }
            public string Op2 { get; set; }
            public string Op3 { get; set; }
            public string Op4 { get; set; }

            public Quadruple(string op, string opr1, string opr2, string result)
            {
                this.Op1 = op;
                this.Op2 = opr1;
                this.Op3 = opr2;
                this.Op4 = result;
            }

            public override string ToString()
            {
                return $"({this.Op1}, {this.Op2}, {this.Op3}, {this.Op4})";
            }
        }

        public class QuadrupleTable
        {
            public List<Quadruple> list;

            public int NXQ
            {
                get { return list.Count; }
            }

            public QuadrupleTable()
            {
                list = new List<Quadruple>();
            }

            public bool Add(string op, string opr1, string opr2, string result)
            {
                this.list.Add(new Quadruple(op, opr1, opr2, result));
                return true; // Assuming add is always successful, adjust as necessary
            }

            public Boolean BackPatch(int index, string result)
            {
                if (index >= 0 && index < list.Count) // Ensuring the index is within valid range
                {
                    list[index].Op4 = result; // Update the fourth element (result) of the specified Quadruple
                    return true;
                }
                return false;
            }


            public void dump(ListBox lb)
            {
                lb.Items.Add("<-------- Mid-Code Table ------------->");
                lb.Items.Add(string.Format("{0,-3} | {1,-4} | {2,-6} | {3,-6} | {4,-6}", "Idx", "Op", "Opr1", "Opr2", "Result"));
                lb.Items.Add(new string('-', 38));
                for (int i = 0; i < list.Count; i++)
                {
                    Quadruple q = list[i];
                    lb.Items.Add(string.Format("{0,-3} | {1,-4} | {2,-6} | {3,-6} | {4,-6}", i, q.Op1, q.Op2, q.Op3, q.Op4));
                }
                lb.Items.Add(new string('-', 38));
            }
        }

        private void debug(string s)
        {
            listBox1.Items.Add(s);
        }
        public List<MidCode> ConvertToMidCodeList()
        {
            List<MidCode> result = new List<MidCode>();
            foreach (var q in midCodes.list)
            {
                result.Add(new MidCode
                {
                    Op = q.Op1,
                    Arg1 = q.Op2,
                    Arg2 = q.Op3,
                    Result = q.Op4
                });
            }
            return result;
        }
        private void HandleParseClick(object sender, EventArgs e)
        {
            sourceProgram = textBox1.Text + "#";
            listBox.Items.Clear();
            listBox1.Items.Clear();
            Identifiers = new IdentifierTable();
            tempVars = new TemporaryVariableTable();
            midCodes = new QuadrupleTable();

            debug(sourceProgram);
            currentPointer = 0;
            currentToken = tokenizer();
            print(currentToken.ToString());

            // Directly use the result of parseProgram in the if condition
            if (parseProgram())
            {
                print("end..");
            }
            else
            {
                print("error...");
            }
            midCodes.dump(listBox);
            Identifiers.dump(this.listBox1);
            tempVars.dump(this.listBox1);
            
            List<MidCode> codeList = ConvertToMidCodeList();
            ExecuteMidCode(codeList);
            listBox1.Items.Add("=== Memory After Execution ===");
            foreach (var item in memory)
            {
                listBox1.Items.Add(item.Key + " = " + item.Value);
            }
        }


        public bool match(string expectedType)
        {
            if (currentToken.type == expectedType)
            {
                debug(string.Concat(new string[]
                {
            "expec ",
            expectedType,
            ", currentToken ",
            currentToken.type,
            ", matched."
                }));
                currentToken = tokenizer();
                print(currentToken.ToString());
                return true;
            }
            else
            {
                print(string.Concat(new string[]
                {
            "Error: expec ",
            expectedType,
            ", got ",
            currentToken.ToString(),
            "."
                }));
                return false;
            }
        }

        public bool parseProgram()
        {
            print("< program > → <variable declaration section> ; <statements section>");
            if (!parseDeclarationSection())
            {
                print("error: Fail to parse <variable declaration section>");
                return false;
            }
            if (!match("semiColon"))
            {
                return false;
            }
            if (!parseStatementsSection())
            {
                print("error: Fail to parse <statements section>");
                return false;
            }
            debug("[< program > → <variable declaration section> ; <statements section>]end");
            return true;
        }

        public bool parseDeclarationSection()
        {
            print("< variable declaration section > → int <variable list>");
            string type = currentToken.value;
            if (!match("kw_int"))
            {
                return false;
            }
            if (!parseVariableList(type))
            {
                print("error: Fail to parse <variable list>");
                return false;
            }
            debug("[< variable declaration section > → int <variable list>]end");
            return true;
        }

        public bool parseVariableList(string type)
        {
            print("< variable list > → identifier A");
            string name = currentToken.value;
            if (!match("identifier"))
            {
                return false;
            }
            if (!Identifiers.add(name))
            {
                print("error: Fail to add identifier " + name);
                return false;
            }
            if (!Identifiers.updateTypeByName(name, type))
            {
                print("error: Fail to update identifier " + name + " with type " + type);
                return false;
            }
            if (!parseA(type))
            {
                print("error: Fail to parse A");
                return false;
            }
            debug("[< variable list > → identifier A]end");
            return true;
        }

        public bool parseA(string type)
        {
            if (currentToken.type == "comma")
            {
                print("A → , identifier A");
                if (!match("comma"))
                {
                    return false;
                }
                string name = currentToken.value;
                if (!match("identifier"))
                {
                    return false;
                }
                if (!Identifiers.add(name))
                {
                    print("error: Fail to add identifier " + name);
                    return false;
                }
                if (!Identifiers.updateTypeByName(name, type))
                {
                    print("error: Fail to update identifier " + name + " with type " + type);
                    return false;
                }
                if (!parseA(type))
                {
                    print("error: Fail to parse A");
                    return false;
                }
                debug("[A → , identifier A]end");
                return true;
            }
            else
            {
                if (currentToken.type == "semiColon")
                {
                    print("A →ε");
                    return true;
                }
                print("can't choose production for parseA with " + currentToken.ToString());
                return false;
            }
        }

        public bool parseStatementsSection()
        {
            print("< statements section > → <statement>; B");
            if (!parseStatement())
            {
                print("error: Fail to parse <statement>");
                return false;
            }
            if (!match("semiColon"))
            {
                return false;
            }
            if (!parseB())
            {
                print("error: Fail to parse B");
                return false;
            }
            debug("[< statements section > → <statement>; B]end");
            return true;
        }

        public bool parseB()
        {
            if (currentToken.type == "identifier" || currentToken.type == "kw_if" || currentToken.type == "kw_while")
            {
                print("B → <statement>; B");
                if (!parseStatement())
                {
                    print("error: Fail to parse <statement>");
                    return false;
                }
                if (!match("semiColon"))
                {
                    return false;
                }
                if (!parseB())
                {
                    print("error: Fail to parse B");
                    return false;
                }
                debug("[B → <statement>; B]end");
                return true;
            }
            else
            {
                if (currentToken.type == "#" || currentToken.type == "kw_end")
                {
                    print("B →ε");
                    return true;
                }
                print("can't choose production for parseB with " + currentToken.ToString());
                return false;
            }
        }

        public bool parseStatement()
        {
            if (currentToken.type == "identifier")
            {
                print("< statement > → <assignment statement>");
                if (!parseAssignmentStatement())
                {
                    print("error: Fail to parse <assignment statement>");
                    return false;
                }
                debug("[< statement > → <assignment statement>]end");
                return true;
            }
            else if (currentToken.type == "kw_if")
            {
                print("< statement > →<conditional statement>");
                if (!parseConditionalStatement())
                {
                    print("error: Fail to parse <conditional statement>");
                    return false;
                }
                debug("[< statement > →<conditional statement>]end");
                return true;
            }
            else
            {
                if (!(currentToken.type == "kw_while"))
                {
                    print("error: can't choose production for parseStatement with " + currentToken.ToString());
                    return false;
                }
                print("< statement > →<iteration statement>");
                if (!parseIterationStatement())
                {
                    print("error: Fail to parse <iteration statement>");
                    return false;
                }
                debug("[< statement > →<iteration statement>]end");
                return true;
            }
        }

        public bool parseAssignmentStatement()
        {
            print("<assignment statement> → identifier = <expression>");

            string name = currentToken.value;
            if (!match("identifier"))
            {
                print("error: Expected 'identifier' but got " + currentToken.value);
                return false;
            }

            if (!match("assign"))
            {
                print("error: Expected '=' after identifier.");
                return false;
            }

            Identifier E = parseExpression();
            if (E == null)
            {
                print("error: Failed to parse <expression>");
                return false;
            }

            if (!Identifiers.updateValueByName(name, E.value))
            {
                print($"error: Cannot update identifier '{name}' with value '{E.value}'");
                return false;
            }

            midCodes.Add("=", E.name, "null", name);
            debug("<assignment statement> → identifier = <expression> end");
            return true;
        }

        public Identifier parseExpression()
        {
            print("<expression> → <item> C");

            Identifier E1 = parseItem();
            if (E1 == null)
            {
                print("error: Failed to parse <item>");
                return null;
            }

            Identifier C = parseC(E1);
            if (C == null)
            {
                print("error: Failed to parse C");
                return null;
            }

            debug("[<expression> → <item> C] end");
            return C;
        }

        public Identifier parseItem()
        {
            print("< item > → <factor> D");

            Identifier E1 = parseFactor();
            if (E1 == null)
            {
                print("error: Failed to parse <factor>");
                return null;
            }

            Identifier D = parseD(E1);
            if (D == null)
            {
                print("error: Failed to parse D");
                return null;
            }

            debug("[< item > → <factor> D] end");
            return D;
        }

        public Identifier parseC(Identifier E1)
        {
            if (currentToken.type == "opPlus")
            {
                print("C -> <item> C");
                if (!match("opPlus"))
                {
                    print("error: Expected '+' but found " + currentToken.value);
                    return null;
                }

                Identifier E2 = parseItem();
                if (E2 == null)
                {
                    print("error: Failed to parse second item in addition");
                    return null;
                }

                Identifier T = tempVars.CreateNewTempVar();
                T.type = E1.type; // Assuming the type remains consistent for simplicity
                try
                {
                    T.value = (Convert.ToInt32(E1.value) + Convert.ToInt32(E2.value)).ToString();
                }
                catch (Exception ex)
                {
                    print("error: Failed to convert or add values: " + ex.Message);
                    return null;
                }

                midCodes.Add("+", E1.name, E2.name, T.name);

                Identifier C = parseC(T);
                if (C == null)
                {
                    print("error: Failed to continue parsing after addition");
                    return null;
                }

                debug("C -> <item> C end");
                return C;
            }
            else if (currentToken.type == "semiColon" || currentToken.type == "rightP" || currentToken.type == "log_op")
            {
                print("C -> ε");
                return E1;
            }

            print("error: Can’t choose production for parseC with " + currentToken.ToString());
            return null;
        }

        public Identifier parseFactor()
        {
            if (currentToken.type == "identifier")
            {
                print("< factor > → identifier");

                string name = currentToken.value;
                if (!match("identifier"))
                {
                    print("error: Expected 'identifier' but found " + currentToken.value);
                    return null;
                }

                Identifier F = Identifiers.getIdentifierByName(name);
                if (F == null)
                {
                    print("error: Identifier '" + name + "' not found");
                    return null;
                }
                if (string.IsNullOrEmpty(F.value))
                {
                    print("error: The value of identifier '" + name + "' is empty");
                    return null;
                }
                debug("[< factor > → identifier] end");
                return F;
            }
            else if (currentToken.type == "integer")
            {
                print("< factor > → integer");

                string value = currentToken.value;
                if (!match("integer"))
                {
                    print("error: Expected 'integer' but found " + currentToken.value);
                    return null;
                }

                Identifier F = new Identifier(value);
                F.type = "int";
                F.value = value;

                debug("[< factor > → integer] end");
                return F;
            }
            else if (currentToken.type == "leftP")
            {
                print("< factor > → ( < expression > )");

                if (!match("leftP"))
                {
                    print("error: Expected '(' but found " + currentToken.value);
                    return null;
                }

                Identifier E = parseExpression();
                if (E == null)
                {
                    print("error: Failed to parse < expression >");
                    return null;
                }

                if (!match("rightP"))
                {
                    print("error: Expected ')' but found " + currentToken.value);
                    return null;
                }

                debug("[< factor > → ( < expression > )] end");
                return E;
            }

            print($"error: Can’t choose production for parseFactor with {currentToken.ToString()}");
            return null;
        }

        public Identifier parseD(Identifier E1)
        {
            if (currentToken.type == "opTime")
            {
                print("D → * < factor > D");
                if (!match("opTime"))
                {
                    print("error: Expected '*' but found " + currentToken.value);
                    return null;
                }

                Identifier E2 = parseFactor();
                if (E2 == null)
                {
                    print("error: Failed to parse < factor >");
                    return null;
                }

                // Got E1 * E2
                Identifier T = tempVars.CreateNewTempVar();
                T.type = E1.type;
                try
                {
                    T.value = (Convert.ToInt32(E1.value) * Convert.ToInt32(E2.value)).ToString();
                }
                catch (Exception ex)
                {
                    print("error: Failed to convert or multiply values: " + ex.Message);
                    return null;
                }
                midCodes.Add("*", E1.name, E2.name, T.name);

                Identifier D = parseD(T);
                if (D == null)
                {
                    print("error: Failed to parse D after multiplication");
                    return null;
                }

                debug("[D → * < factor > D] end");
                return D;
            }
            else if (currentToken.type == "semiColon" || currentToken.type == "rightP" || currentToken.type == "log_op" || currentToken.type == "opPlus")
            {
                print("D → ε");
                return E1;
            }

            print("error: Can’t choose production for parseD with " + currentToken.ToString());
            return null;
        }

        public bool parseConditionalStatement()
        {
            print("<conditional statement> → if (<condition>) then <statement> [else <statement>]");

            if (!match("kw_if"))
                return false;

            if (!match("leftP"))
                return false;

            Identifier T = parseCondition();
            if (T == null)
            {
                print("error: Fail to parse condition");
                return false;
            }

            // 🔥 IF TRUE → go to THEN
            // 🔥 IF TRUE → go to THEN
            midCodes.Add("jnz", T.name, "null", (midCodes.NXQ + 2).ToString());

            // store condition index for re-evaluation
            int condIndex = midCodes.NXQ - 1;

            // 🔥 IF FALSE → jump to ELSE
            int falseJump = midCodes.NXQ;
            midCodes.Add("j", "null", "null", "0");

            if (!match("rightP"))
                return false;

            if (!match("kw_then"))
                return false;

            // ✅ THEN part
            if (!parseStatement())
                return false;

            // consume semicolon before else
            if (currentToken.type == "semiColon")
                match("semiColon");

            // 🔥 jump over ELSE
            int endJump = midCodes.NXQ;
            midCodes.Add("j", "null", "null", "0");

            // 🔥 fix FALSE → go to ELSE
            midCodes.BackPatch(falseJump, midCodes.NXQ.ToString());

            // ✅ ELSE part (optional)
            if (currentToken.type == "kw_else")
            {
                match("kw_else");

                if (!parseStatement())
                    return false;
            }

            // 🔥 fix END
            midCodes.BackPatch(endJump, midCodes.NXQ.ToString());

            debug("[if (<condition>) then <statement> [else <statement>]] end");

            return true;
        }

        public Identifier parseCondition()
        {
            print("<expression> logical_operator <expression>");

            // Parse the first expression
            Identifier E1 = parseExpression();
            if (E1 == null)
            {
                print("error: Failed to parse the first expression");
                return null;
            }

            // Get the logical operator
            string op = currentToken.value;
            if (!match("log_op"))
            {
                print("error: Expected a logical operator but found " + currentToken.value);
                return null;
            }

            // Parse the second expression
            Identifier E2 = parseExpression();
            if (E2 == null)
            {
                print("error: Failed to parse the second expression");
                return null;
            }

            // Create a temporary variable for the result of the condition
            Identifier T = tempVars.CreateNewTempVar();
            T.type = "bool";

            // Evaluate the logical operation and store the result
            if (string.IsNullOrEmpty(E1.value) || string.IsNullOrEmpty(E2.value))
            {
                print($"error: Value(s) empty - E1: {E1.value}, E2: {E2.value}");
                return null;
            }

            // Handling different operators
            T.value = "false"; // runtime will evaluate this

            // Add the result of the evaluation to the middle code table
            midCodes.Add(op, E1.name, E2.name, T.name);
            debug($"[<expression> {op} <expression>] end");

            return T;
        }

        public bool parseNestedStatement()
        {
            if (currentToken.type == "identifier" || currentToken.type == "kw_if" || currentToken.type == "kw_while")
            {
                print("< nested statement > → <statement>");
                if (!parseStatement())
                {
                    return false;
                }
                debug("[< nested statement > → <statement>]end");
                return true;
            }
            else if (currentToken.type == "kw_begin")
            {
                print("< nested statement > → <compound statement>");
                if (!parseCompoundStatement())
                {
                    return false;
                }
                debug("[< nested statement > → <compound statement>]end");
                return true;
            }
            else
            {
                print("can't choose production for < nested statement > with " + currentToken.ToString());
                return false;
            }
        }

        public bool parseCompoundStatement()
        {
            print("< compound statement > → begin < statements section > end");

            // Check if 'begin' keyword matches
            if (!match("kw_begin"))
            {
                return false;
            }

            // Attempt to parse the statements section
            if (!parseStatementsSection())
            {
                return false;
            }

            // Check if 'end' keyword matches
            if (!match("kw_end"))
            {
                return false;
            }

            // If all checks pass, debug and return true
            debug("[< compound statement > → begin < statements section > end]end");
            return true;
        }

        public bool parseIterationStatement()
        {
            print("while (<condition>) do <nested statement>");

            // while
            if (!match("kw_while"))
            {
                print("error: Expected 'while'");
                return false;
            }

            // (
            if (!match("leftP"))
            {
                print("error: Expected '('");
                return false;
            }

            int start = midCodes.NXQ;   // 🔥 loop start position

            // condition
            Identifier T = parseCondition();
            if (T == null)
            {
                print("error: condition failed");
                return false;
            }

            // if true → go inside loop
            midCodes.Add("jnz", T.name, "null", (midCodes.NXQ + 2).ToString());

            // if false → exit loop (to be patched)
            int falseJump = midCodes.NXQ;
            midCodes.Add("j", "null", "null", "0");

            // )
            if (!match("rightP"))
            {
                print("error: Expected ')'");
                return false;
            }

            // do
            if (!match("kw_do"))
            {
                print("error: Expected 'do'");
                return false;
            }

            // body
            if (!parseNestedStatement())
            {
                print("error: nested statement failed");
                return false;
            }

            // 🔁 jump back to loop start
            midCodes.Add("j", "null", "null", start.ToString());

            // 🔧 fix false jump (exit)
            midCodes.BackPatch(falseJump, midCodes.NXQ.ToString());

            debug("[while (...) do ...] end");
            return true;
        }
        public class MidCode
        {
            public string Op;
            public string Arg1;
            public string Arg2;
            public string Result;
        }

        public void ExecuteMidCode(List<MidCode> midCodes)
        {
            int pc = 0;
            memory.Clear();

            while (pc < midCodes.Count)
            {
                MidCode code = midCodes[pc];

                switch (code.Op)
                {
                    case "=":
                        SetValue(code.Result, GetValue(code.Arg1));
                        pc++;
                        break;

                    case "+":
                        SetValue(code.Result, Convert.ToInt32(GetValue(code.Arg1)) + Convert.ToInt32(GetValue(code.Arg2)));
                        pc++;
                        break;

                    case "*":
                        SetValue(code.Result, Convert.ToInt32(GetValue(code.Arg1)) * Convert.ToInt32(GetValue(code.Arg2)));
                        pc++;
                        break;

                    case ">":
                        SetValue(code.Result, Convert.ToInt32(GetValue(code.Arg1)) > Convert.ToInt32(GetValue(code.Arg2)));
                        pc++;
                        break;

                    case "<":
                        SetValue(code.Result, Convert.ToInt32(GetValue(code.Arg1)) < Convert.ToInt32(GetValue(code.Arg2)));
                        pc++;
                        break;

                    case ">=":
                        SetValue(code.Result, Convert.ToInt32(GetValue(code.Arg1)) >= Convert.ToInt32(GetValue(code.Arg2)));
                        pc++;
                        break;

                    case "<=":
                        SetValue(code.Result, Convert.ToInt32(GetValue(code.Arg1)) <= Convert.ToInt32(GetValue(code.Arg2)));
                        pc++;
                        break;

                    case "jnz":
                        if (Convert.ToBoolean(GetValue(code.Arg1)))
                            pc = Convert.ToInt32(code.Result);
                        else
                            pc++;
                        break;

                    case "j":
                        pc = Convert.ToInt32(code.Result);
                        break;

                    default:
                        pc++;
                        break;
                }
            }
        }

        Dictionary<string, object> memory = new Dictionary<string, object>();

        object GetValue(string name)
        {
            if (memory.ContainsKey(name))
                return memory[name];

            // if it's a number
            if (int.TryParse(name, out int v))
                return v;

            if (bool.TryParse(name, out bool b))
                return b;

            return 0;
        }

        void SetValue(string name, object value)
        {
            if (memory.ContainsKey(name))
                memory[name] = value;
            else
                memory.Add(name, value);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // leave empty
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "CodeFlow Compiler";
            this.BackColor = Color.FromArgb(10, 10, 30);
        }

    }  // ✅ THIS closes your class

}
/// BACKUP OF Parsing Mechanism 
/*        private void HandleParseClick(object sender, EventArgs e)
{
    sourceProgram = textBox1.Text + "#";
    listBox.Items.Clear();
    listBox1.Items.Clear();
    debug(sourceProgram);
    currentPointer = 0;
    currentToken = tokenizer();
    print(currentToken.ToString());

    // Directly use the result of parseProgram in the if condition
    if (parseProgram())
    {
        print("end..");
    }
    else
    {
        print("error...");
    }
}

/// <summary>
/// Attempts to match the current token's type with the expected type.
/// This function is a key component in ensuring the parser's current state aligns with the grammar expectations.
/// If the current token matches the expected type, the function logs this match, fetches the next token,
/// and returns true, indicating successful progression in the parsing process.
/// If there is a mismatch, the function logs an error with details of the expectation versus the actual token,
/// and returns false, indicating a parsing error at this point.
///
/// Usage: This method is used extensively in parsing functions to validate the syntax of the structured
/// input according to predefined grammatical rules.
///
/// Example:
/// If the parser expects an identifier and the current token is indeed an identifier, the match is successful.
/// If the current token is not an identifier, the match fails, and the function logs the discrepancy.
/// </summary>
/// <param name="expectedType">The type of token expected at this point in the parse process.</param>
/// <returns>Boolean indicating whether the current token matches the expected type.</returns>

public bool match(string expectedType)
{
    if (currentToken.type == expectedType)
    {
        debug(string.Concat(new string[]
        {
    "expec ",
    expectedType,
    ", currentToken ",
    currentToken.type,
    ", matched."
        }));
        currentToken = tokenizer();
        print(currentToken.ToString());
        return true;
    }
    else
    {
        print(string.Concat(new string[]
        {
    "Error: expec ",
    expectedType,
    ", got ",
    currentToken.ToString(),
    "."
        }));
        return false;
    }
}


/// <summary>
/// Initiates the parsing process for the entire program. This method orchestrates the parsing by calling
/// specific methods that parse the variable declaration section followed by the statements section,
/// separated by a semicolon. This reflects the grammar rule:
/// <program> -> <variable declaration section> ; <statements section>
/// The method returns true if both sections are parsed successfully and are syntactically correct,
/// otherwise returns false indicating a syntax error.
/// </summary>
/// <returns>Boolean indicating the overall syntax correctness of the program.</returns>
public bool parseProgram()
{
    print("< program > → <variable declaration section> ; <statements section>");

    // Check if declaration section is parsed correctly
    if (!parseDeclarationSection())
    {
        return false;
    }

    // Check if the semicolon is correctly matched
    if (!match("semiColon"))
    {
        return false;
    }

    // Check if statements section is parsed correctly
    if (!parseStatementsSection())
    {
        return false;
    }

    // If all checks passed, debug and return true
    debug("[< program > → <variable declaration section> ; <statements section>]end");
    return true;
}



/// <summary>
/// Parses the variable declaration section of the program based on the grammar:
/// <variable declaration section> -> int <variable list>
/// This method checks for the 'int' keyword followed by a list of variables. It uses `match()`
/// to ensure 'int' is present and then calls `parseVariableList()` to handle the variable list.
/// Returns true if the section adheres to the grammar, otherwise false.
/// </summary>
/// <returns>Boolean indicating if the declaration section is correctly parsed.</returns>
public bool parseDeclarationSection()
{
    print("< variable declaration section > → int <variable list>");

    // Check if the keyword "int" is matched correctly
    if (!match("kw_int"))
    {
        return false;
    }

    // Check if the variable list is parsed correctly
    if (!parseVariableList())
    {
        return false;
    }

    // If all checks passed, debug and return true
    debug("[< variable declaration section > → int <variable list>]end");
    return true;
}


/// <summary>
/// Parses a list of variables from the input based on the grammar:
/// <variable list> -> identifier A
/// This method is responsible for checking if the list starts with a valid identifier and
/// then handles any subsequent identifiers separated by commas using the helper method `parseA()`.
/// Returns true if the list matches the expected format, otherwise false.
/// </summary>
/// <returns>Boolean indicating if the variable list is correctly parsed.</returns>
/// 
public bool parseVariableList()
{
    print("< variable list > → identifier A");

    // Check if the "identifier" token matches correctly
    if (!match("identifier"))
    {
        return false;
    }

    // Check if the A part of the variable list is parsed correctly
    if (!parseA())
    {
        return false;
    }

    // If all checks passed, debug and return true
    debug("[< variable list > → identifier A]end");
    return true;
}

/// <summary>
/// Parses the continuation of a variable list after the first identifier, handling commas and
/// additional identifiers based on the grammar:
/// A -> , identifier A | ε
/// It checks for a comma followed by another identifier, recursively calling itself to process
/// further identifiers. If a semicolon is encountered, it signifies the end of the list, returning true.
/// Returns false if the expected tokens are not found, indicating a syntax error.
/// </summary>
/// <returns>Boolean indicating if the continuation of the variable list is correctly parsed.</returns>

public bool parseA()
{
    if (currentToken.type == "comma")
    {
        print("A → , identifier A");
        if (!match("comma"))
        {
            return false;
        }
        if (!match("identifier"))
        {
            return false;
        }
        if (!parseA())
        {
            return false;
        }
        debug("[A → , identifier A]end");
        return true;
    }
    else if (currentToken.type == "semiColon")
    {
        print("A →ε");
        return true;
    }
    else
    {
        print("can't choose production for parseA with " + currentToken.ToString());
        return false;
    }
}


/// <summary>
/// Parses the statements section of the program, which consists of one or more statements
/// separated by semicolons, based on the grammar:
/// <statements section> -> <statement> ; B
/// This method processes each statement using `parseStatement()` and expects a semicolon
/// after each statement, using `parseB()` to handle additional statements.
/// Returns true if all statements are correctly parsed according to the grammar, otherwise false.
/// </summary>
/// <returns>Boolean indicating if the statements section is correctly parsed.</returns>

public bool parseStatementsSection()
{
    print("< statements section > → <statement>; B");

    // Check if the statement is parsed correctly
    if (!parseStatement())
    {
        return false;
    }

    // Check if the semicolon is matched correctly
    if (!match("semiColon"))
    {
        return false;
    }

    // Check if the B part of the statements section is parsed correctly
    if (!parseB())
    {
        return false;
    }

    // If all checks passed, debug and return true
    debug("[< statements section > → <statement>; B]end");
    return true;
}


/// <summary>
/// Handles the recursive parsing of additional statements in the statements section based on the grammar:
/// B -> <statement> ; B | ε
/// This method attempts to parse another statement if the current token allows, checking for
/// a semicolon as a separator and recursively calling itself for further statements.
/// It returns true when no more statements are present or if all recursive calls are successful.
/// </summary>
/// <returns>Boolean indicating if the continuation of statements is correctly parsed.</returns>

public bool parseB()
{
    if (currentToken.type == "identifier" || currentToken.type == "kw_if" || currentToken.type == "kw_while")
    {
        print("B → <statement>; B");
        if (!parseStatement())
        {
            return false;
        }
        if (!match("semiColon"))
        {
            return false;
        }
        if (!parseB())
        {
            return false;
        }
        debug("[B → <statement>; B]end");
        return true;
    }
    else if (currentToken.type == "#" || currentToken.type == "kw_end")
    {
        print("B →ε");
        return true;
    }
    else
    {
        print("can't choose production for parseB with " + currentToken.ToString());
        return false;
    }
}

/// <summary>
/// Parses a single statement, which can be an assignment, conditional, or iteration statement based on:
/// <statement> -> <assignment statement> | <conditional statement> | <iteration statement>
/// This method determines the type of statement to parse by examining the current token and
/// delegates to the specific parsing method for that statement type.
/// Returns true if the statement is parsed successfully according to the relevant grammar rules, otherwise false.
/// </summary>
/// <returns>Boolean indicating if a statement is correctly parsed.</returns>
public bool parseStatement()
{
    if (currentToken.type == "identifier")
    {
        print("< statement > → <assignment statement>");
        if (!parseAssignmentStatement())
        {
            return false;
        }
        debug("[< statement > → <assignment statement>]end");
        return true;
    }
    else if (currentToken.type == "kw_if")
    {
        print("< statement > → <conditional statement>");
        if (!parseConditionalStatement())
        {
            return false;
        }
        debug("[< statement > → <conditional statement>]end");
        return true;
    }
    else if (currentToken.type == "kw_while")
    {
        print("< statement > → <iteration statement>");
        if (!parseIterationStatement())
        {
            return false;
        }
        debug("[< statement > → <iteration statement>]end");
        return true;
    }
    else
    {
        print("can't choose production for parseStatement with " + currentToken.ToString());
        return false;
    }
}

/// <summary>
/// Parses an assignment statement based on the grammar:
/// <assignment statement> → identifier = <expression>
/// This method sequentially verifies that the current token is an identifier, followed by an assignment operator,
/// and finally attempts to parse an expression. It ensures that each component matches the expected grammar,
/// logging the process and returning true on success. If any part fails, it immediately returns false.
/// </summary>
/// <returns>Boolean indicating whether the assignment statement is correctly parsed.</returns>
public bool parseAssignmentStatement()
{
    print("< assignment statement > → identifier = <expression>");

    // Attempt to match the identifier
    if (!match("identifier"))
    {
        return false;
    }

    // Attempt to match the assignment operator
    if (!match("assign"))
    {
        return false;
    }

    // Attempt to parse the expression
    if (!parseExpression())
    {
        return false;
    }

    // If all steps are successful, log debug information and return true
    debug("[< assignment statement > → identifier = <expression>]end");
    return true;
}

/// <summary>
/// Parses an expression based on the grammar:
/// <expression> → <item> C
/// This method involves parsing an item and then handling any additional components of the expression
/// as defined by the parseC() method. It returns true if both components are successfully parsed, otherwise false.
/// </summary>
/// <returns>Boolean indicating whether the expression is correctly parsed.</returns>

public bool parseExpression()
{
    print("< expression > → <item> C");

    // Attempt to parse an item
    if (!parseItem())
    {
        return false;
    }

    // Attempt to parse C
    if (!parseC())
    {
        return false;
    }

    // If both steps are successful, log debug information and return true
    debug("[< expression > → <item> C]end");
    return true;
}

/// <summary>
/// Parses an item in the expression based on the grammar:
/// <item> → <factor> D
/// This function checks if the factor is parsed correctly and then proceeds to parse any subsequent factors
/// using parseD(). It returns true if the entire item is successfully parsed according to the grammar, otherwise false.
/// </summary>
/// <returns>Boolean indicating whether the item is correctly parsed.</returns>
public bool parseItem()
{
    print("< item > → <factor> D");

    // Check if the factor is parsed correctly
    if (!parseFactor())
    {
        return false;
    }

    // Check if D is parsed correctly
    if (!parseD())
    {
        return false;
    }

    // If both checks passed, debug and return true
    debug("[< item > → <factor> D]end");
    return true;
}

/// <summary>
/// Parses additional components of an expression handling operators and recursion based on the grammar:
/// C → + <item> C | ε
/// It checks for a plus operator and recursively parses additional items or terminates if no further operators are found.
/// This method returns true if the parse sequence is correct, including proper recursion and termination, otherwise false.
/// </summary>
/// <returns>Boolean indicating whether the continuation of the expression is correctly parsed.</returns>


public bool parseC()
{
    if (currentToken.type == "opPlus")
    {
        print("C → + <item> C");
        if (!match("opPlus"))
        {
            return false;
        }
        if (!parseItem())
        {
            return false;
        }
        if (!parseC())
        {
            return false;
        }
        debug("[C → + <item> C]end");
        return true;
    }
    else if (currentToken.type == "semiColon" || currentToken.type == "rightP" || currentToken.type == "log_op")
    {
        print("C →ε");
        return true;
    }
    else
    {
        print("can't choose production for parseC with " + currentToken.ToString());
        return false;
    }
}

/// <summary>
/// Parses a factor, which can be an identifier, a numeric integer, or a nested expression, based on the grammar:
/// <factor> → identifier | integer | (<expression>)
/// This method determines the type of factor based on the current token and processes it accordingly,
/// returning true if the factor is correctly parsed, otherwise false.
/// </summary>
/// <returns>Boolean indicating whether the factor is correctly parsed.</returns>


public bool parseFactor()
{
    if (currentToken.type == "identifier")
    {
        print("< factor > → identifier ");
        if (!match("identifier"))
        {
            return false;
        }
        debug("[< factor > → identifier]end");
        return true;
    }
    else if (currentToken.type == "integer")
    {
        print("< factor > → integer ");
        if (!match("integer"))
        {
            return false;
        }
        debug("[< factor > → integer]end");
        return true;
    }
    else if (currentToken.type == "leftP")
    {
        print("< factor > → (< expression >) ");
        if (!match("leftP"))
        {
            return false;
        }
        if (!parseExpression())
        {
            return false;
        }
        if (!match("rightP"))
        {
            return false;
        }
        debug("[< factor > → (< expression >)]end");
        return true;
    }
    else
    {
        print("can't choose production for parseFactor with " + currentToken.ToString());
        return false;
    }
}


/// <summary>
/// Parses the multiplication part of an expression handling factors and recursion based on the grammar:
/// D → * <factor> D | ε
/// It checks for a multiplication operator and recursively parses additional factors, or terminates if no further operators are found.
/// This method returns true if the multiplication sequence is correct, including proper recursion and termination, otherwise false.
/// </summary>
/// <returns>Boolean indicating whether the continuation of multiplication in the expression is correctly parsed.</returns>


public bool parseD()
{
    if (currentToken.type == "opTime")
    {
        print("D → * < factor > D");
        if (!match("opTime"))
        {
            return false;
        }
        if (!parseFactor())
        {
            return false;
        }
        if (!parseD())
        {
            return false;
        }
        debug("[D → * < factor > D]end");
        return true;
    }
    else if (currentToken.type == "semiColon" || currentToken.type == "rightP" || currentToken.type == "log_op" || currentToken.type == "opPlus")
    {
        print("D →ε");
        return true;
    }
    else
    {
        print("can't choose production for parseD with " + currentToken.ToString());
        return false;
    }
}

/// <summary>
/// Parses a conditional statement based on the grammar:
/// <conditional statement> → if (<condition>) then <nested statement> ; else <nested statement>
/// This function ensures each part of the if-else structure is present and correctly formatted, including
/// the condition and both possible execution paths. Returns true if the entire structure is valid, otherwise false.
/// </summary>
/// <returns>Boolean indicating whether the conditional statement is correctly parsed.</returns>

public bool parseConditionalStatement()
{
    print("< conditional statement > → if （< condition >） then <nested statement> ; else < nested statement > ");

    if (!match("kw_if"))
    {
        return false;
    }
    if (!match("leftP"))
    {
        return false;
    }
    if (!parseCondition())
    {
        return false;
    }
    if (!match("rightP"))
    {
        return false;
    }
    if (!match("kw_then"))
    {
        return false;
    }
    if (!parseNestedStatement())
    {
        return false;
    }
    if (!match("semiColon"))
    {
        return false;
    }
    if (!match("kw_else"))
    {
        return false;
    }
    if (!parseNestedStatement())
    {
        return false;
    }

    debug("[< conditional statement > → if （< condition >） then <nested statement> ; else < nested statement > ]end");
    return true;
}


/// <summary>
/// Parses a logical condition which is part of conditional and loop constructs, based on the grammar:
/// <condition> → <expression> logical_operator <expression>
/// This involves parsing two expressions separated by a logical operator. It ensures both expressions
/// and the operator are correctly handled and returns true on success, otherwise false.
/// </summary>
/// <returns>Boolean indicating whether the condition is correctly parsed.</returns>


public bool parseCondition()
{
    print("< expression > logical_operator < expression >");

    // Parse the first expression
    if (!parseExpression())
    {
        return false;
    }

    // Match the logical operator
    if (!match("log_op"))
    {
        return false;
    }

    // Parse the second expression
    if (!parseExpression())
    {
        return false;
    }

    debug("[< expression > logical_operator < expression >]end");
    return true;
}


/// <summary>
/// Parses a nested statement, which can be either a simple statement or a compound statement wrapped by 'begin' and 'end',
/// based on the grammar:
/// <nested statement> → <statement> | <compound statement>
/// This method decides which type of statement to parse based on the current token and processes accordingly,
/// returning true if the nested statement is correctly parsed, otherwise false.
/// </summary>
/// <returns>Boolean indicating whether the nested statement is correctly parsed.</returns>


public bool parseNestedStatement()
{
    if (currentToken.type == "identifier" || currentToken.type == "kw_if" || currentToken.type == "kw_while")
    {
        print("< nested statement > → <statement>");
        if (!parseStatement())
        {
            return false;
        }
        debug("[< nested statement > → <statement>]end");
        return true;
    }
    else if (currentToken.type == "kw_begin")
    {
        print("< nested statement > → <compound statement>");
        if (!parseCompoundStatement())
        {
            return false;
        }
        debug("[< nested statement > → <compound statement>]end");
        return true;
    }
    else
    {
        print("can't choose production for < nested statement > with " + currentToken.ToString());
        return false;
    }
}


/// <summary>
/// Parses a compound statement, typically used in block structures of control flow, based on the grammar:
/// <compound statement> → begin <statements section> end
/// This function ensures that the 'begin' keyword starts the block, follows it with a series of statements,
/// and ends with the 'end' keyword. It returns true if the block is correctly formed, otherwise false.
/// </summary>
/// <returns>Boolean indicating whether the compound statement is correctly parsed.</returns>


public bool parseCompoundStatement()
{
    print("< compound statement > → begin < statements section > end");

    // Check if 'begin' keyword matches
    if (!match("kw_begin"))
    {
        return false;
    }

    // Attempt to parse the statements section
    if (!parseStatementsSection())
    {
        return false;
    }

    // Check if 'end' keyword matches
    if (!match("kw_end"))
    {
        return false;
    }

    // If all checks pass, debug and return true
    debug("[< compound statement > → begin < statements section > end]end");
    return true;
}


/// <summary>
/// Parses an iteration (loop) statement based on the grammar:
/// <iteration statement> → while (<condition>) do <nested statement>
/// This method checks for the 'while' keyword followed by a condition and a nested statement,
/// ensuring each component is present and correctly formatted. Returns true if the loop structure
/// is valid, otherwise false.
/// </summary>
/// <returns>Boolean indicating whether the iteration statement is correctly parsed.</returns>


public bool parseIterationStatement()
{
    print("while (< condition >) do < nested statement >");

    // Check if 'while' keyword matches
    if (!match("kw_while"))
    {
        return false;
    }

    // Check if the left parenthesis matches
    if (!match("leftP"))
    {
        return false;
    }

    // Attempt to parse the condition
    if (!parseCondition())
    {
        return false;
    }

    // Check if the right parenthesis matches
    if (!match("rightP"))
    {
        return false;
    }

    // Check if 'do' keyword matches
    if (!match("kw_do"))
    {
        return false;
    }

    // Attempt to parse the nested statement
    if (!parseNestedStatement())
    {
        return false;
    }

    // If all checks pass, debug and return true
    debug("[while (< condition >) do < nested statement >]end");
    return true;
}*/