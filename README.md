Compiled and ran with `mcs lispish.cs && mono lispish.exe`. Test cases in main.

Example Output:
```
==================================================
Input: (define foo 3)
--------------------------------------------------
Tokens
--------------------------------------------------
LITERAL                 : (
ID                      : define
ID                      : foo
INT                     : 3
LITERAL                 : )
--------------------------------------------------
Parse Tree
--------------------------------------------------
Program                                    
  SExpr                                    
    List                                   
      LITERAL                              (
      Seq                                  
        SExpr                              
          Atom                             
            ID                             define
        Seq                                
          SExpr                            
            Atom                           
              ID                           foo
          Seq                              
            SExpr                          
              Atom                         
                INT                        3
      LITERAL                              )
--------------------------------------------------
```
