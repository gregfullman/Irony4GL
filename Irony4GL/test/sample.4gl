# This is the working test example for Informix 4gl parsing
#database
#global
define x bigint   # required

main

    define y double(3)   # required
    
    #defer quit          # OR
    #codeblock
    

end main

#required
#reportDef      # OR
function functionId (param1 para2)

    define z decimal(3,2)   #required
    define rec record like tableId.*
    define arr array[3] of int
    
    #simple expression - assignment
    let variable = expression
    
    # simple expression - call
    call functionId returning variable
    
    # structured expression
    structuredStatement
    
    
end function