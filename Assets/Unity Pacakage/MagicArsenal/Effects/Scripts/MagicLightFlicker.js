// Properties
var waveFunction : String = "sin"; // possible values: sin, tri(angle), sqr(square), saw(tooth), inv(verted sawtooth), noise (random)
var base : float = 0.0; // start
var amplitude : float = 1.0; // amplitude of the wave
var phase : float = 0.0; // start point inside on wave cycle
var frequency : float = 0.5; // cycle frequency per second
 
// Keep a copy of the original color
private var originalColor : Color;
 
// Store the original color
function Start () {
    originalColor = GetComponent(Light).color;
}
 
function Update () {
    var light : Light = GetComponent(Light);
    light.color = originalColor * (EvalWave());
}
 
function EvalWave () {
    var x : float = (Time.time + phase)*frequency;
    var y : float;
 
    x = x - Mathf.Floor(x); // normalized value (0..1)
 
    if (waveFunction=="sin") {
        y = Mathf.Sin(x*2*Mathf.PI);
    }
    else if (waveFunction=="tri") {
        if (x < 0.5)
            y = 4.0 * x - 1.0;
        else
            y = -4.0 * x + 3.0;  
    }    
    else if (waveFunction=="sqr") {
        if (x < 0.5)
            y = 1.0;
        else
            y = -1.0;  
    }    
    else if (waveFunction=="saw") {
        y = x;
    }    
    else if (waveFunction=="inv") {
        y = 1.0 - x;
    }    
    else if (waveFunction=="noise") {
        y = 1 - (Random.value*2);
    }
    else {
        y = 1.0;
    }        
    return (y*amplitude)+base;     
}
 