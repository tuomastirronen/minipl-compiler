# MiniPL-compiler


## Sample programs

```
var X : int := 4 + (6 * 2);
print X;
```

```
var nTimes : int := 0;
print "How many times?";
read nTimes;
var x : int;
for x in 0..nTimes-1 do
  print x;
  print " : Hello, World!\n";
end for;
assert (x = nTimes);
```

```
print "Give a number";
var n : int;
read n;
var v : int := 1;
var i : int;
for i in 1..n do
  v := v * i;
end for;
print "The result is: ";
print v;
 ```
 
 ```
print "\n**************";       /* box top */
var x : int;
for x in 1..8 do
    print "\n*            *";   /* box sides   */
end for;
print "\n**************\n";     /* bottom of the box */
```

## Context-free grammar

#### prog
* stmts

#### stmts
* stmt ';' [ stmt ';' ]

#### stmt 
* 'var' var_ident ':' type [ ':=' expr ]
* var_ident **:=** expr
* 'for' var_ident 'in' expr '..' expr 'do' stmts 'end' 'for'
* 'read' var_ident
* 'print' expr
* 'assert' '(' expr ')'

#### expr
* opnd op opnd
* [ unary_op ] opnd
 
#### opnd 
* int
* string
* var_ident
* '(' expr ')'

#### type
* 'int'
* 'string'
* 'bool'

#### var_ident
* ident

#### reserved keyword
* 'var'
* 'for'
* 'end'
* 'in'
* 'do'
* 'read'
* 'print'
* 'int'
* 'string'
* 'bool'
* 'assert'
