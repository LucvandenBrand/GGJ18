import {Scene, OrthographicCamera, WebGLRenderer, Mesh} from 'three';
import VoronoiRenderer from './voronoiRenderer'

document.addEventListener("DOMContentLoaded", function()
{
	/* Setup the main environment. */
	const scene = new Scene();
	const renderer = new WebGLRenderer();
	renderer.setSize(window.innerWidth, window.innerHeight);
	document.body.appendChild(renderer.domElement);

	var width = 4;
	var height = window.innerHeight / window.innerWidth * width;
	const camera = new OrthographicCamera(width / -2, width / 2,
		                                    height / 2, height / -2,
																				0.1, 1000);
  const voronoiRenderer = new VoronoiRenderer(renderer, camera);

  /* Main render function. */
	function render()
	{
		requestAnimationFrame(render);
		renderer.render(scene, camera);
	}

	render();
});
