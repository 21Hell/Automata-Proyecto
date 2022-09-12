using System;
using System.Collections.Generic;

//Requerimiento 1.- eliminar las dobles comillas del printf e interpretar 
//las secuencias de escape dentro de la cadena Completado


//Requerimiento 2.- Marcar los errores Sintacticos cuando la variable no exista 
//Completado
//Si no existe la variable (get contenido) levantamos una escepcion 


//Requerimiento 3.- Modificar el valor de la variable en la asignacion   Completado

//Requerimineto 4.- Obtener el valor de la variable cuando se requiera y programar el metodo getValor   Printf Cadena o expresion  Completado


//Requerimeinto 5 .- Modificar el valor de la variable en scanf   Completado
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
        private string limpiarPrints(string sucio){
            string cleaned = sucio.TrimStart('"');
            cleaned = cleaned.Remove(cleaned.Length-1);
            cleaned = cleaned.Replace(@"\n", "\n");
            cleaned = cleaned.Replace(@"\t", "\t");
            return cleaned;
        }
        private void addVariable(String nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {
            log.WriteLine();
            log.WriteLine("Variables: ");
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

        private float getValor(string nombre){
            float valorV = 0;
            foreach (Variable v in variables)
            {
                if(v.getNombre().Equals(nombre)){
                valorV = v.getValor();
                }
            }
            return valorV;
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
            }else{
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
            string variable = getContenido();
            if (existeVariable(getContenido())){
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
                modVariable(variable,getValor(variable)+1);
            }
            else if (getContenido() == "--") 
            {
                match("--");
                modVariable(variable,getValor(variable)-1);
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
            stack.Pop();
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
                }else{
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
            stack.Pop();
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
            stack.Pop();
            match(Tipos.OperadorRelacional);
            Expresion();
            stack.Pop();
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
            if(getClasificacion() == Tipos.Cadena){
                string str = getContenido();
                string cleaned = limpiarPrints(str);
                Console.Write(cleaned);
                match(Tipos.Cadena);
            }else{
                Expresion();
                Console.Write(stack.Pop());
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &Identificador );
        private void Scanf()
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            if (existeVariable(getContenido())){
                string NombreVariable = getContenido();
                match(Tipos.Identificador);
                string val = ""+Console.ReadLine();
                float valorFloat = float.Parse(val);
                Console.Write(valorFloat);
                modVariable(NombreVariable,valorFloat);
                match(")");
                match(";");
            }else{
                throw new Error("Error de syntaxis: variable no declarada: <"+getContenido()+"> en linea  "+linea, log);
            }
            
            
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
                log.Write(" ");
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (existeVariable(getContenido())){
                    log.Write(getContenido());
                    log.Write(" ");
                    stack.Push(getValor(getContenido()));
                    match(Tipos.Identificador);
                }else{
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