##Se recomienda utilizar postMan, ya que he desactivado las configuraciones para swagger

## Explicación de la Situación Actual:
Actualmente, se ha identificado un problema en la deserialización de la propiedad EmployerLengthMonths de la clase LoanRequest.
Aunque el XML proporcionado contiene claramente un valor de "12" para este atributo, 
durante el proceso de deserialización, se observa que el valor de EmployerLengthMonths es asignado como cero.

## Posible Causa del Problema:
A pesar de haber intentado diferentes enfoques, como el uso de los atributos XmlElement y XmlAttribute, 
así como asegurarse de que el tipo de datos sea el correcto, la causa raíz del problema aún no ha sido identificada.

## Acciones Realizadas:
Se han realizado varias acciones para abordar este problema, incluyendo la revisión de la definición de la clase LoanRequest, 
la reinicialización de la aplicación y la verificación del código XML antes de la deserialización. 
Sin embargo, hasta el momento, el problema persiste.

## Validaciones Implementadas:
Es importante destacar que las validaciones del XML se han implementado en la clase LoanValidator que se encuentra en la carpeta Validator. 
Este componente verifica múltiples aspectos del XML para garantizar que cumpla con los requisitos especificados, a pesar que he comentado
las validaciones debido al error anteriormente mencionado, he decido comentar por limitaciones de tiempo.

## Limitaciones de Tiempo:
Debido a restricciones de tiempo actuales, no es posible profundizar más en la resolución de este problema. 
Se reconoce que la deserialización incorrecta de EmployerLengthMonths es un inconveniente, y se planea abordar este problema en 
futuras iteraciones del desarrollo. De todas formas me mantrende trabajando para resolver este problema, 
si me brindan mas tiempo puedo corregirlo

## Pasos Siguientes Recomendados:
Para resolver completamente este problema, se recomienda dedicar más tiempo a la depuración y revisión detallada del código. 
Además, se podría considerar la asistencia de colegas o recursos adicionales para obtener una perspectiva fresca sobre el problema
por eso la importancia del trabajo en equipo, considere que estoy trabajando solo.