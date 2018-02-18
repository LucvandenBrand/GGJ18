const webpack = require('webpack'),
    path = require('path'),
    ExtractTextPlugin = require("extract-text-webpack-plugin"),
    HtmlWebpackPlugin = require("html-webpack-plugin");

var inProduction = process.env.NODE_ENV === 'production';

// Retrieve the version string from package.json
function getVersion()
{
	return require("./package.json").version;
}

// Retrieve the name string from package.json
function getName()
{
	return require("./package.json").name;
}


module.exports = {
    entry: {
		display: './src/main.js'
    },
    output: {
        path: path.join(__dirname, './dist'),
        filename: '[name]-' + getVersion() + '.js'
    },
	module: {
	 rules: [
           {
	     test: /\.css$/,
             use: ExtractTextPlugin.extract({
                  use: 'css-loader'})
	   },
           {
             test: /\.glsl$/,
             use: 'raw-loader'
           }
         ],
	 loaders: [
		 {
			 test: /\.js$/,
			 loader: 'babel-loader',
			 query: {
				 presets: ['es2015']
			 }
		 }
	 ]
	},
	plugins: [
          new ExtractTextPlugin('css/[name]-' + getVersion() + '.css'),
          new HtmlWebpackPlugin({title: getName() + '-' + getVersion(), filename: 'index.html'})
        ],
	stats: {
		colors: true
	},
	devtool: 'source-map'
};

if(inProduction) {
	// Do production specific things here.
}
