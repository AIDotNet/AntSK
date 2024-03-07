async function embedChatbot() {
	const chatBtnId = 'aiagent-chatbot-button'
	const chatWindowId = 'aiagent-chatbot-window'
	const script = document.getElementById('antsk-iframe')
	const botSrc = script?.getAttribute('data-src')
	const width = script?.getAttribute('data-width') || '30rem'
	const height = script?.getAttribute('data-height') || '50rem'
	const primaryColor = script?.getAttribute('data-color') || '#4e83fd'
	const defaultOpen = script?.getAttribute('data-default-open') === 'true'
	const MessageIconUrl = script?.getAttribute('data-message-icon-url')

	if (!botSrc) {
		console.error(`Can't find chaturl`)
		return
	}
	if (document.getElementById(chatBtnId)) {
		return
	}

	const MessageIcon = `<?xml version="1.0" encoding="UTF-8"?>
	<svg width="48px" height="50px" viewBox="0 0 48 50" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
	 <g id="页面-1" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
			<g id="编组-6" transform="translate(0.000000, 1.000000)">
				<rect id="矩形" fill="#FFFFFF" opacity="0" x="0" y="0" width="48" height="49.7454545"></rect>
				<g id="zhinengxuexi-5" transform="translate(0.000000, 1.000000)" fill-rule="nonzero">
					<path d="M39.4407295,31.1732523 L39.4407295,17.3860182 C39.4407295,16.1702128 38.7355623,15.0273556 37.6899696,14.3221884 L25.7264438,7.44072948 C24.6808511,6.83282675 23.2948328,6.83282675 22.224924,7.44072948 L10.3100304,14.3465046 C9.26443769,14.9544073 8.55927052,16.0972644 8.55927052,17.4103343 L8.55927052,31.2705167 C8.55927052,32.4863222 9.26443769,33.6291793 10.3100304,34.3343465 L22.2735562,41.1428571 C23.3191489,41.7507599 24.7051672,41.7507599 25.775076,41.1428571 L37.7386018,34.2613982 C38.8328267,33.6291793 39.4407295,32.4863222 39.4407295,31.1732523 Z" id="路径" fill="#D5ECFF"></path>
					<path d="M28.7416413,31.7082067 L24.7051672,31.7082067 L23.3920973,27.768997 L17.118541,27.768997 L15.8054711,31.7082067 L11.768997,31.7082067 L18.0425532,14.443769 L22.3465046,14.443769 L28.7416413,31.7082067 L28.7416413,31.7082067 Z M22.662614,24.8510638 L20.6930091,18.8449848 C20.5957447,18.4802432 20.4984802,17.993921 20.4012158,17.337386 L20.3039514,17.337386 C20.3039514,17.7993921 20.1094225,18.3586626 20.0121581,18.8449848 L18.0425532,24.8510638 L22.662614,24.8510638 L22.662614,24.8510638 Z M34.6504559,14.3465046 L34.6504559,31.6109422 L31.0030395,31.6109422 L31.0030395,14.3465046 L34.6504559,14.3465046 Z" id="形状" fill="#027FE6"></path>
					<path d="M44.5957447,29.0820669 C43.8905775,29.0820669 43.2826748,29.6899696 43.2826748,30.3951368 L43.2826748,34.0668693 C43.2826748,34.8449848 42.8449848,35.550152 42.1398176,35.9878419 L25.3130699,45.6656535 C24.6079027,46.1033435 23.7325228,46.1033435 23.0516717,45.6656535 L6.22492401,35.9878419 C5.51975684,35.6474164 5.08206687,34.8449848 5.08206687,34.0668693 L5.08206687,29.2036474 C6.32218845,28.6443769 7.17325228,27.4042553 7.17325228,25.9452888 C7.17325228,23.9756839 5.56838906,22.3708207 3.59878419,22.3708207 C1.62917933,22.3708207 0,23.9756839 0,25.9452888 C0,27.5258359 1.0212766,28.8632219 2.43161094,29.325228 L2.43161094,34.0425532 C2.43161094,35.7933131 3.37993921,37.3495441 4.86322188,38.224924 L21.6899696,48 C22.4680851,48.43769 23.2705167,48.6079027 24.1215805,48.6079027 C24.899696,48.6079027 25.775076,48.3404255 26.5531915,47.9027356 L43.3069909,38.224924 C44.7902736,37.3495441 45.7386018,35.7933131 45.7386018,34.0425532 L45.7386018,30.3708207 C45.8115502,29.6170213 45.2765957,29.0820669 44.5957447,29.0820669 Z M45.9088146,18.7720365 L45.9088146,14.5167173 C45.9088146,12.7659574 44.9604863,11.2097264 43.4772036,10.3343465 L26.6261398,0.656534954 C25.1428571,-0.218844985 23.2218845,-0.218844985 21.7386018,0.656534954 L4.88753799,10.3343465 C3.40425532,11.2097264 2.45592705,12.7659574 2.45592705,14.5167173 L2.45592705,17.8237082 C2.45592705,18.5288754 3.06382979,19.1367781 3.76899696,19.1367781 C4.47416413,19.1367781 5.08206687,18.5288754 5.08206687,17.8237082 L5.08206687,14.5167173 C5.08206687,13.7386018 5.51975684,13.0334347 6.22492401,12.5957447 L23.0516717,2.91793313 C23.7568389,2.48024316 24.6322188,2.48024316 25.3130699,2.91793313 L42.0668693,12.6930091 C42.7720365,13.0334347 43.2097264,13.8358663 43.2097264,14.6139818 L43.2097264,18.674772 C41.8480243,19.1854103 40.8510638,20.4741641 40.8510638,22.0303951 C40.8510638,24 42.4559271,25.6048632 44.4255319,25.6048632 C46.3951368,25.6048632 48,24 48,22.0303951 C48,20.5714286 47.1246201,19.331307 45.9088146,18.7720365 Z" id="形状" fill="#027FE6"></path>
				</g>
			</g>
		</g>
	</svg>`

	const CloseIcon = `<?xml version="1.0" standalone="no"?><!DOCTYPE svg PUBLIC "-//W3C//DTD SVG 1.1//EN" "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd"><svg t="1690535441526" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="6367" xmlns:xlink="http://www.w3.org/1999/xlink"><path d="M512 1024A512 512 0 1 1 512 0a512 512 0 0 1 0 1024zM305.956571 370.395429L447.488 512 305.956571 653.604571a45.568 45.568 0 1 0 64.438858 64.438858L512 576.512l141.604571 141.531429a45.568 45.568 0 0 0 64.438858-64.438858L576.512 512l141.531429-141.604571a45.568 45.568 0 1 0-64.438858-64.438858L512 447.488 370.395429 305.956571a45.568 45.568 0 0 0-64.438858 64.438858z" fill=${primaryColor} p-id="6368"></path></svg>`
	const MessageIconImg = document.createElement('img')
	MessageIconImg.src = MessageIconUrl
	MessageIconImg.style.cssText = 'width: 100%; height: 100%;'
	const ChatBtn = document.createElement('div')
	ChatBtn.id = chatBtnId
	ChatBtn.style.cssText = 'position: fixed; bottom: 1rem; right: 1rem; width: 40px; height: 40px; cursor: pointer; z-index: 2147483647; transition: 0;'

	const ChatBtnDiv = document.createElement('div')
	if (MessageIconUrl) {
		ChatBtnDiv.appendChild(MessageIconImg)
	} else {
		ChatBtnDiv.innerHTML = MessageIcon
	}
	/**
	 * 添加样式
	 */
	// 创建一个style元素
	const style = document.createElement('style')
	style.type = 'text/css'

	// 添加CSS规则到style元素
	const rules = document.createTextNode('.resizing-iframe iframe { pointer-events: none; }' + '.resizing-iframe { user-select: none; }')

	// 将规则附加到style元素
	style.appendChild(rules)

	// 将style元素添加到document的head中，使其生效
	document.head.appendChild(style)
	const parentDiv = document.createElement('div')
	parentDiv.style.cssText = `border: none; position: fixed; flex-direction: column; justify-content: space-between; box-shadow: rgba(150, 150, 150, 0.2) 0px 10px 30px 0px, rgba(150, 150, 150, 0.2) 0px 0px 0px 1px; bottom: 4rem; right: 1rem; min-width:${width}; min-height: ${height};width:${width}; height: ${height}; max-width: 90vw; max-height: 85vh; border-radius: 0.75rem; display: flex; z-index: 2147483647; overflow: hidden; left: unset; background-color: #F3F4F6;`
	parentDiv.id = chatWindowId
	parentDiv.style.visibility = defaultOpen ? 'unset' : 'hidden'
	const iframe = document.createElement('iframe')
	iframe.allow = 'fullscreen;microphone'
	iframe.title = 'AIAgent Chat Window'
	iframe.target = '_self'
	// iframe.id = chatWindowId
	iframe.src = botSrc
	iframe.style.cssText = `width: 100%; height: 100%; border: none;`
	parentDiv.appendChild(iframe)
	document.body.appendChild(parentDiv)
	let resizing = false
	let startX, startWidth

	const resizer = document.createElement('div')
	resizer.className = 'resizer'
	iframe.parentElement.appendChild(resizer) // 假设iframe和resizer有相同的父元素

	// 为resizer添加样式
	resizer.style.width = '2px'
	resizer.style.height = '100%'
	resizer.style.background = '#f8f9fd'
	resizer.style.position = 'absolute'
	resizer.style.left = '0'
	resizer.style.bottom = '0'
	resizer.style.cursor = 'e-resize'

	// 监听鼠标按下事件
	resizer.addEventListener('mousedown', (e) => {
		e.preventDefault()
		document.body.classList.add('resizing-iframe')
		startX = e.clientX
		// startY = e.clientY
		startWidth = parseInt(document.defaultView.getComputedStyle(parentDiv).width, 10)
		// startHeight = parseInt(document.defaultView.getComputedStyle(parentDiv).height, 10)
		resizing = true
	})

	// 监听鼠标移动事件
	document.addEventListener('mousemove', (e) => {
		if (!resizing) return
		requestAnimationFrame(() => {
			let newWidth = startWidth + (startX - e.clientX)
			// let newHeight = startHeight + (e.clientY - startY)
			parentDiv.style.width = newWidth + 'px'
			// parentDiv.style.height = newHeight + 'px'
		})
	})

	// 监听鼠标释放事件
	document.addEventListener('mouseup', (e) => {
		document.body.classList.remove('resizing-iframe')
		resizing = false
	})

	let chatBtnDragged = false
	let chatBtnDown = false
	let chatBtnMouseX
	let chatBtnMouseY
	ChatBtn.addEventListener('click', function () {
		if (chatBtnDragged) {
			chatBtnDragged = false
			return
		}
		const chatWindow = document.getElementById(chatWindowId)

		if (!chatWindow) return
		const visibilityVal = chatWindow.style.visibility
		if (visibilityVal === 'hidden') {
			chatWindow.style.visibility = 'unset'
			ChatBtnDiv.innerHTML = CloseIcon
		} else {
			chatWindow.style.visibility = 'hidden'
			if (MessageIconUrl) {
				ChatBtnDiv.innerHTML = ''
				ChatBtnDiv.appendChild(MessageIconImg)
			} else {
				ChatBtnDiv.innerHTML = MessageIcon
			}
		}
	})

	ChatBtn.addEventListener('mousedown', (e) => {
		if (!chatBtnMouseX && !chatBtnMouseY) {
			chatBtnMouseX = e.clientX
			chatBtnMouseY = e.clientY
		}

		chatBtnDown = true
	})
	ChatBtn.addEventListener('mousemove', (e) => {
		if (!chatBtnDown) return
		chatBtnDragged = true
		const transformX = e.clientX - chatBtnMouseX
		const transformY = e.clientY - chatBtnMouseY

		ChatBtn.style.transform = `translate3d(${transformX}px, ${transformY}px, 0)`

		e.stopPropagation()
	})
	ChatBtn.addEventListener('mouseup', (e) => {
		chatBtnDown = false
	})
	ChatBtn.addEventListener('mouseleave', (e) => {
		chatBtnDown = false
	})

	ChatBtn.appendChild(ChatBtnDiv)
	document.body.appendChild(ChatBtn)
}
document.body.onload = embedChatbot
