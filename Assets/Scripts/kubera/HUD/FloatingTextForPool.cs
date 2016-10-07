using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 * Clase creada para evitar el uso de Tweens en los textos del dinero que dan los edificios.
 * Este texto es manipulado por una pool que verifica el estado de los textos para saber si esta disponible o no.
 */
public class FloatingTextForPool : FloatingTextBase 
{
	protected FloatingTextPool pool;
	
	/*protected override void Start()
	{
		base.Start();
	}*/

	public void setPool(FloatingTextPool _pool)
	{
		pool = _pool;
		OnEnded += pool.releaseText;
	}

	/*
	 * La funcion mueve el texto al centro del edificiio, desde donde se inicia su animacion.
	 * 
	 * @params start{Vector3}: El punto de inicio de la animacion, el texto se mueve en un inicio a esta posicion
	 * 
	 * @params end{Vector3}: El punto en el que se termina la animacion, y se libera el texto para ser reutilizado
	 */
	/*public override void startAnim(Vector3 start,Vector3 end)
	{
		base.startAnim (start,end);
	}*/
}