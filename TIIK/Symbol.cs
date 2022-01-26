using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIIK
{
    class Symbol
    {
        private int symbol;
        private int instances;
        private double probility;
        private string code;

        public Symbol(int symbol)
        {
            this.symbol = symbol;
            instances = 0;
            probility = 0;
            code = "";
        }

        public int getCodeLenght()
        {
            return code.Length;
        }

        public int getSymbol()
        {
            return this.symbol;
        }

        public void setSymbol(int symbol)
        {
            this.symbol = symbol;
        }

        public int getInstances()
        {
            return this.instances;
        }

        public void setInstances()
        {
            this.instances = this.instances + 1;
        }

        public double getProbility()
        {
            return this.probility;
        }

        public void setProbility(double probility)
        {
            this.probility = probility;
        }

        public string getCode()
        {
            return this.code;
        }

        public void setCode(char bit)
        {
            this.code = this.code + bit;
        }
    }
}
