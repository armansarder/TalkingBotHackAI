class StressSmash {
    constructor() {
        this.canvas = document.getElementById('gameCanvas');
        this.ctx = this.canvas.getContext('2d');
        this.score = 0;
        this.problemsSmashed = 0;
        this.stressItems = [];
        this.isLebronMode = false;
        this.isSlowMode = false;
        
        // Set canvas size
        this.canvas.width = 800;
        this.canvas.height = 600;
        
        // Cursor customization
        this.canvas.style.cursor = 'none';
        
        // Bind event listeners
        this.canvas.addEventListener('mousemove', this.handleMouseMove.bind(this));
        this.canvas.addEventListener('click', this.handleClick.bind(this));
        document.getElementById('lebronMode').addEventListener('click', this.activateLebronMode.bind(this));
        document.getElementById('deepBreath').addEventListener('click', this.activateSlowMode.bind(this));
        document.getElementById('themeToggle').addEventListener('click', this.toggleTheme.bind(this));
        
        // Initialize sounds
        this.smashSound = document.getElementById('smashSound');
        this.lebronQuote = document.getElementById('lebronQuote');
        
        // Start game loop
        this.gameLoop();
        
        // Spawn stress items periodically
        setInterval(() => this.spawnStressItem(), 2000);
    }
    
    handleMouseMove(e) {
        const rect = this.canvas.getBoundingClientRect();
        this.mouseX = e.clientX - rect.left;
        this.mouseY = e.clientY - rect.top;
    }
    
    handleClick(e) {
        const rect = this.canvas.getBoundingClientRect();
        const clickX = e.clientX - rect.left;
        const clickY = e.clientY - rect.top;
        
        // Check for collisions with stress items
        this.stressItems = this.stressItems.filter(item => {
            const distance = Math.hypot(clickX - item.x, clickY - item.y);
            const hitRadius = this.isLebronMode ? 50 : 30;
            
            if (distance < hitRadius) {
                this.score += 100;
                this.problemsSmashed++;
                this.playSmashEffect(item.x, item.y);
                return false;
            }
            return true;
        });
    }
    
    spawnStressItem() {
        const stressTypes = ['Deadline', 'Traffic', 'Karen'];
        const item = {
            x: Math.random() * (this.canvas.width - 50),
            y: -50,
            type: stressTypes[Math.floor(Math.random() * stressTypes.length)],
            speed: this.isSlowMode ? 1 : 3
        };
        this.stressItems.push(item);
    }
    
    activateLebronMode() {
        if (this.score >= 1000) {
            this.score -= 1000;
            this.isLebronMode = true;
            this.lebronQuote.play();
            setTimeout(() => this.isLebronMode = false, 10000);
        }
    }
    
    activateSlowMode() {
        if (this.score >= 500) {
            this.score -= 500;
            this.isSlowMode = true;
            setTimeout(() => this.isSlowMode = false, 5000);
        }
    }
    
    playSmashEffect(x, y) {
        this.smashSound.currentTime = 0;
        this.smashSound.play();
        
        // Create particle effect
        this.ctx.fillStyle = '#ff4444';
        this.ctx.beginPath();
        this.ctx.arc(x, y, 20, 0, Math.PI * 2);
        this.ctx.fill();
    }
    
    toggleTheme() {
        const themes = ['#f0f0f0', '#2c3e50', '#2ecc71'];
        document.body.style.background = themes[(themes.indexOf(document.body.style.background) + 1) % themes.length];
    }
    
    gameLoop() {
        // Clear canvas
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Update stress items
        this.stressItems = this.stressItems.filter(item => {
            item.y += item.speed;
            
            // Draw stress item
            this.ctx.fillStyle = '#ff4444';
            this.ctx.beginPath();
            this.ctx.arc(item.x, item.y, 20, 0, Math.PI * 2);
            this.ctx.fill();
            
            this.ctx.fillStyle = '#000';
            this.ctx.font = '12px Arial';
            this.ctx.fillText(item.type, item.x - 20, item.y);
            
            return item.y < this.canvas.height;
        });
        
        // Draw custom cursor
        if (this.mouseX && this.mouseY) {
            this.ctx.fillStyle = '#333';
            this.ctx.beginPath();
            this.ctx.moveTo(this.mouseX, this.mouseY);
            this.ctx.lineTo(this.mouseX + 20, this.mouseY + 20);
            this.ctx.lineTo(this.mouseX + 5, this.mouseY + 30);
            this.ctx.closePath();
            this.ctx.fill();
        }
        
        // Update score display
        document.getElementById('score').textContent = this.score;
        document.getElementById('problems').textContent = this.problemsSmashed;
        
        requestAnimationFrame(() => this.gameLoop());
    }
}

// Start the game when the page loads
window.onload = () => new StressSmash();
