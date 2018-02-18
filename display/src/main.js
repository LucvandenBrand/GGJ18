import {Scene, OrthographicCamera, WebGLRenderer, ConeGeometry, ShaderMaterial, Mesh} from 'three';
import voronoiVertex from './shaders/voronoiVertex.glsl'
import voronoiFragment from './shaders/voronoiFragment.glsl'

document.addEventListener("DOMContentLoaded", function()
{
	const scene = new Scene();
        var width = 4;
        var height = window.innerHeight / window.innerWidth * width;
	const camera = new OrthographicCamera(width / -2, width / 2, height / 2, height / -2, 0.1, 1000);

	const renderer = new WebGLRenderer();
	renderer.setSize(window.innerWidth, window.innerHeight);
	document.body.appendChild(renderer.domElement);

	const geometry = new ConeGeometry(2, 2, 32);
	const material = new ShaderMaterial(
        {
          vertexShader   : voronoiVertex,
          fragmentShader : voronoiFragment
        });

	const cube = new Mesh(geometry, material);
	scene.add(cube);

	camera.position.z = 10;

	function render()
	{
		requestAnimationFrame(render);
		cube.rotation.x += 0.1;
		cube.rotation.y += 0.1;
		cube.rotation.z += 0.1;
		renderer.render(scene, camera);
	}

	render();
});


