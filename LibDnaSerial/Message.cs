namespace LibDnaSerial
{
    /// <summary>
    /// Base class for messages to and from 
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Message type code
        /// 
        /// See https://github.com/hobbyquaker/dna-commands for possible values
        /// </summary>
        public char Code { get; protected set; }

        /// <summary>
        /// Message argument (part after the equals "=" sign)
        /// </summary>
        public string Argument { get; protected set; }

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="code">Populate the Code property</param>
        /// <param name="argument">Populate the Argument property</param>
        public Message(char code, string argument)
        {
            Code = code;
            Argument = argument;
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", Code, Argument);
        }
    }
}
