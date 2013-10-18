# This is the working test example for Informix 4gl parsing
#database
#global
define x bigint   # required

main

    define y double(3)   # required
    
    #defer quit          # OR
    let y = 3.14
    

end main

#required
#reportDef      # OR
function functionId (param1 para2)

    define z decimal(3,2)   #required
    define rec record like tableId.*
    define arr array[3] of int
    
    #simple expression - assignment
    let variable = variable + 1
    
    # simple expression - call
    call programname returning variable
    
    # structured expression
    if (variable = 1 or arr[44] = 1) then
        let variable = -2 * 4
    else 
        let arr[44] = 2
    end if
    
    
end function