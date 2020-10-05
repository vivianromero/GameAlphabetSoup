# Juego Sopa de letras
Lenguaje utilizado C#.

##  `Sopa de letras`

Se creó una engine que genere aleatoriamente una sopa de letras, con la posibilidad de que estas palabras estén 
ubicadas horizontalmente, verticalmente y en diagonal de izquierda a derecha o  derecha a izquierda, de arriba 
hacia abajo o de abajo hacia arriba.

Un parámetro a tener en cuenta en la generación, es que la sopa de letras puede ser de distintos tamaños tanto en 
ancho como en alto sin necesidad de que la misma sea cuadrada, esto dos valores (ancho y alto) no pueden ser menores 
a 15 letras y mayores a 80.

Se interactuar con el engine para poder indicarle toda la parametrización, tanto las alternativas válidas para 
acomodar las palabras como el tamaño de la sopa de letras.

Una vez generada la sopa de letras hay que poder visualizar la misma en forma de caracteres de la siguiente manera

   `|h|s|d|f|g|
	|a|o|d|f|g|
	|a|s|l|f|g|
	|a|s|d|a|g|`
	
Una vez que se pueda visualizar la sopa de letras hay que poder indicarle al sistema que ha encontrado una 
palabra: indicando el par fila y columna inicial de la palabra y el par final; el sistema tiene que poder 
indicar si la palabra es correcta y si el path de la palabra es correcto. Si la palabra es correcta hay que 
transformar los caracteres a su correspondientes mayúsculas.

   `|H|s|d|f|g|
	|a|O|d|f|g|
	|a|s|L|f|g|
	|a|s|d|A|g|`

## Cómo usarlo
    
   En la carpeta `\bin\Debug\` se encuentra un .exe `SoupGame.exe` 
   y el fichero `unixdict.txt` que contiene las palabras.
   
   En el directorio raíz se encuentra el fichero `Program.cs` con el código fuente de la 
   aplicación

## Qué se necesita
+  Visual Studio 2017, Visual Studio Code o un editor de texto.

