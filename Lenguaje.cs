using System;
using System.Collections.Generic;

//Requerimiento 1.- eliminar las dobles comillas del printf e interpretar 
//las secuencias de escape dentro de la cadena Completado
//Requerimiento 2.- Marcar los errores Sintacticos cuando la variable no exista
//Si no existe la variable (get contenido) levantamos una escepcion 
//Requerimiento 3.- Modificar el valor de la variable en la asignacion 
namespace evalua
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();

        Stack<float> stack = new Stack<float>();
        
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

        }
        private void addVariable(String nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre()+" "+v.getTipo()+" "+v.getValor());
            }
        }


        private bool existeVariable(string nombre){
            foreach (Variable v in variables){
                if(v.getNombre().Equals(nombre)){
                    return true;
                }
            }
            return false;
        }


        private void modVariable(string nombre, float nuevoValor){
                foreach (Variable v in variables){
                    if (v.getNombre().Equals(nombre))
                    {
                        v.setValor(nuevoValor);
                    }
                }
        }



        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            Libreria();
            Variables();
            Main();
            displayVariables();
        }

        //Librerias -> #includethrow new Error("Error lexico: No definido en linea: "+linea, log);<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                //if (getContenido() == "#")
                Libreria();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;
                switch (getContenido()){
                    case "int":tipo = Variable.TipoDato.Int;break;
                    case "float":tipo = Variable.TipoDato.Float;break;
                }
                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

        //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Tipos.Identificador)
            {
                if(!existeVariable(getContenido())){
                    addVariable(getContenido(),tipo);
                }else{
                    throw new Error("Error de syntaxis: variable duplicada: <"+getContenido()+"> en linea  "+linea, log);
                }
                   
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Bloque de instrucciones -> {ListaIntrucciones?}
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase()
        {
            Instruccion();
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase();
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "printf")
            {
                Printf();
            }
            else if (getContenido() == "scanf")
            {
                Scanf();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            else if (getContenido() == "switch")
            {
                Switch();
            }
            else
            {
                Asignacion();
            }
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion()
        {
            log.WriteLine();
            log.Write(getContenido()+" = ");
            string NombreVar = getContenido();
            if (existeVariable(getContenido())){
                match(Tipos.Identificador);
                match(Tipos.Asignacion);
                Expresion();
                match(";");
                float resultado = stack.Pop();
                log.Write("= "+resultado);
                log.WriteLine();
                modVariable(NombreVar,resultado);
            }else{
                throw new Error("Error de syntaxis: variable no declarada: <"+getContenido()+"> en linea  "+linea, log);
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do()
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            if (existeVariable(getContenido())){
            match(Tipos.Identificador);
            if (getContenido() == "+")
            {
                match("++");
            }
            else
            {
                match("--");
            }
            }else
            {
                throw new Error("Error de syntaxis: variable no declarada: <"+getContenido()+"> en linea  "+linea, log);
            }
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch()
        {
            match("switch");
            match("(");
            Expresion();
            match(")");
            match("{");
            ListaDeCasos();
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos()
        {
            match("case");
            Expresion();
            match(":");
            ListaInstruccionesCase();
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos();
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private void Condicion()
        {
            Expresion();
            match(Tipos.OperadorRelacional);
            Expresion();
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
        }

        //Printf -> printf(cadena);
        private void Printf()
        {
            match("printf");
            match("(");
            //Requerimineto 1 TERMINADO 
            string str = getContenido();
            string cleaned = str.Trim('"');
            //if(cleaned contains //n)
            if(cleaned.Contains("\n")){
                Console.WriteLine(cleaned.Replace("\\n", ""));
            }else{
                Console.Write(cleaned);
            }
            match(Tipos.Cadena);
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena);
        private void Scanf()
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(")");
            match(";");
        }

        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones();
        }

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador+" ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();


                switch (operador){
                    case "+":
                        stack.Push(n2+n1);
                    break;
                    case "-":
                        stack.Push(n2-n1);
                    break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador+"  ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador){
                    case "*":
                        stack.Push(n2*n1);
                    break;
                    case "/":
                        stack.Push(n2/n1);
                    break;
                }


            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido());
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (existeVariable(getContenido())){
                    match(Tipos.Identificador);
                    }else
                    {
                        throw new Error("Error de syntaxis: variable no declarada: <"+getContenido()+"> en linea  "+linea, log);
                    }
                
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}