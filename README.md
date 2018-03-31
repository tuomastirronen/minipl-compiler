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
print "\n**************";
var x : int;
for x in 1..8 do
    print "\n*            *";
end for;
print "\n**************\n";
```

## Usage

    dotnet run <your_program>
