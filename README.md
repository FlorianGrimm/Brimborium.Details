# Brimborium.Details

Technical documentation build with relation to the source code.
So you can document your reasons / desitions / ideas and link it with the source code.

## Goal

Collects Information from the source code.
Modify markdown files.


VS Code: extension for link details

for files
details://path/file.ext:line

for marked locations
details://Header1/Header2/Label1 
opens markdown or opens source code

### Exsample

```CS
class Abc{
    public void Do(){
        // § Header1 / Header2 / Label 1
        var x = new Xyz();
        x.Something();
    }
}
class Xyz{
    public void Something(){
        // § Header1 / Header2 / Label 2
    }
}
```

```MD
# Header1

## Header2

§> Show-List

- Label1 details://example.cs#4
- Label2 details://example.cs#5


```

## Syntax

### Location

A hierachical location - like unix filenames

1) Filename.md / Header1 / ... / HeaderN
2) Header1 / ... / HeaderN

Rule: the filename version wins if not unique

### Code §-Marker

SingleLine Comment // for C# and TypeScript
Future -- for SQL

§ Location § Order § Short Description §

§ [^§/]+ (/ [^§/]+ )* (§ [^0-9]+)? (§ [^§]+) §?

### Tags
after a §-Marker you can provide additional informations
§ Tag: Value §?

§ [^§:]+ : [^§]+ §?

### Commands

```
§>CommandName Argument* §
```

Argument seperator WhiteSpace or §

§>[-A-Z]+ ([ §]+ ([^ §]+) ([ §]+[^ §]+))? §

#### Show-List
```
§>Show-List Scope?
```
Result is a list of locaion that starts with the given location

Argument: Scope - Default Location
Target element: unordered List (-); ordered List (1. )
Item Format:
- Location details://file:line
  Short Description that is given from the marker
  Long Description that should be not touched from the tool.

#### Show-Call
```
§>Show-Call  Location or C# Method or TypeScript Function
```
Argument: Location
Marked Location in the Code
C# Method Namespace.TypeName.Method or TypeName.Method 
TypeScript Function 
Target element: CodeBlock
Item Format:
mermaid graph TD

#### Show-Diagram
§>Show-Diagram Location
Think of



### Header special
```
# Header
details://file#line
Short Description
```
if # Header1..6 flowed by a line starting with details://
and there is a §-marker in the source code
the link will be updated - if the §-marker is defined multiple times then a list of links is shown - sorted by the filename+line.
if a Short Description is present it will be shown next (distinct for many §-marker)